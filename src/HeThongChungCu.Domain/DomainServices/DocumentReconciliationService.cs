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
                .Where(id => tepTaiLieuDict.ContainsKey(id))
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
}
