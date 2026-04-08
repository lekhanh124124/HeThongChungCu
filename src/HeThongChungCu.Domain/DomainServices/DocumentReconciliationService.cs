using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.DomainServices;

public class DocumentReconciliationService : IDocumentReconciliationService
{
    public void ReconcileNguoiDungDocuments(
        NguoiDung user,
        IEnumerable<DocumentSyncItem> proposedDocs,
        IEnumerable<TepTaiLieu> fetchedFiles)
    {
        var currentDocs = user.TaiLieu.ToList();
        var proposedList = proposedDocs.ToList();

        // 1. Remove documents not in the request
        var proposedOriginalIds = proposedList
            .Where(d => d.Id.HasValue)
            .Select(d => d.Id!.Value)
            .ToList();

        foreach (var doc in currentDocs)
        {
            if (!proposedOriginalIds.Contains(doc.Id))
            {
                user.RemoveDocument(doc.Id);
            }
        }

        // 2. Map pre-fetched files by ID for quick lookup
        var tepTaiLieuDict = fetchedFiles.ToDictionary(f => f.Id);

        // 3. Update existing or Add new
        foreach (var propDoc in proposedList)
        {
            var loaiGiayTo = LoaiGiayTo.FromValue(propDoc.LoaiGiayToId)!;

            // Map file Ids to file entities, casting to the correct sub-type or creating a new one if it's missing the discriminator
            var files = propDoc.FileIds
                .Where(tepTaiLieuDict.ContainsKey)
                .Select(id => tepTaiLieuDict[id])
                .Select(f => f is TepTaiLieuNguoiDung tp
                    ? tp
                    : new TepTaiLieuNguoiDung(f.FileName, f.FileUrl, f.Size, f.ContentType))
                .ToList();

            if (propDoc.Id.HasValue)
            {
                // Update existing document
                var existingDoc = user.TaiLieu.FirstOrDefault(d => d.Id == propDoc.Id.Value);
                if (existingDoc != null)
                {
                    existingDoc.UpdateInfo(loaiGiayTo, propDoc.SoGiayTo, propDoc.NgayPhatHanh?.DateTime);
                    existingDoc.SyncFiles(files);
                }
            }
            else
            {
                // Add new document
                var newDoc = new TaiLieuNguoiDung(
                    user.Id,
                    loaiGiayTo,
                    propDoc.SoGiayTo,
                    propDoc.NgayPhatHanh?.DateTime,
                    files);
                user.AddDocument(newDoc);
            }
        }
    }

    public void ReconcilePhuongTienImages(PhuongTien phuongTien, IEnumerable<TepTaiLieu> hinhAnhs)
    {
        var fetchedList = hinhAnhs.ToList();
        var currentList = phuongTien.HinhAnhPhuongTiens.ToList();

        // 1. Remove files not in the fetched list
        var fetchedIds = fetchedList.Select(f => f.Id).ToHashSet();
        foreach (var current in currentList)
        {
            if (!fetchedIds.Contains(current.Id))
            {
                phuongTien.RemoveHinhAnh(current.Id);
            }
        }

        // 2. Add files not in the current list
        var currentIds = currentList.Select(f => f.Id).ToHashSet();
        foreach (var fetched in fetchedList)
        {
            if (!currentIds.Contains(fetched.Id))
            {
                var newFile = fetched is TepPhuongTien tp
                    ? tp
                    : new TepPhuongTien(fetched.FileName, fetched.FileUrl, fetched.Size, fetched.ContentType, phuongTien.Id);

                phuongTien.AddHinhAnh(newFile);
            }
        }
    }

    public void ReconcileDoiTacHopDongs(
        DoiTac doiTac,
        IEnumerable<HopDongSyncItem> proposedHopDongs,
        IEnumerable<TepTaiLieu> fetchedFiles)
    {
        var proposedList = proposedHopDongs.ToList();
        var currentHopDongs = doiTac.HopDongs.ToList();
        var fileDict = fetchedFiles.ToDictionary(f => f.Id);

        // 1. Only Add new contracts. Updates and Deletes are not allowed via reconciliation.
        foreach (var prop in proposedList)
        {
            if (prop.Id.HasValue)
            {
                // Ignore existing contracts to prevent unauthorized updates.
                continue;
            }

            // Map file Ids to file entities
            var teps = (prop.TepFileIds ?? Enumerable.Empty<int>())
                .Where(fileDict.ContainsKey)
                .Select(id => fileDict[id])
                .Select(f => f is TepHopDongDoiTac th
                    ? th
                    : new TepHopDongDoiTac(f.FileName, f.FileUrl, f.Size, f.ContentType))
                .ToList();

            var newHopDong = new HopDongDoiTac(doiTac.Id, prop.SoHopDong, prop.NgayKy, prop.NgayHetHan, prop.GiaTri, prop.DichVuId, prop.NoiDung);
            newHopDong.SyncTepHopDongs(teps);
            doiTac.AddHopDong(newHopDong);
        }

        // 3. Finalize all contracts statuses
        doiTac.CheckActiveHopDongs();
    }
}
