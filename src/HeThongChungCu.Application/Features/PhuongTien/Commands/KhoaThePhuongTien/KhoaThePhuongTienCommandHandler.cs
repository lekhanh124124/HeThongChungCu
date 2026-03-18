namespace HeThongChungCu.Application.Features.PhuongTien.Commands.KhoaThePhuongTien
{
    public class KhoaThePhuongTienCommandHandler : ICommandHandler<KhoaThePhuongTienCommand, bool>
    {
        private readonly IPhuongTienEFRepository _phuongTienEFRepository;

        public KhoaThePhuongTienCommandHandler(IPhuongTienEFRepository phuongTienEFRepository)
        {
            _phuongTienEFRepository = phuongTienEFRepository;
        }

        public async Task<Result<bool>> Handle(KhoaThePhuongTienCommand request, CancellationToken cancellationToken)
        {
            var phuongTiens = await _phuongTienEFRepository.GetPhuongTiensByTheIdsAsync(request.TheIds, cancellationToken);

            if (!phuongTiens.Any())
                return Result.Failure<bool>(PhuongTienErrors.NotFound);

            foreach (var theId in request.TheIds)
            {
                var phuongTien = phuongTiens.FirstOrDefault(x => x.ThePhuongTiens.Any(t => t.Id == theId));
                if (phuongTien != null)
                {
                    phuongTien.KhoaThe(theId);
                    _phuongTienEFRepository.Update(phuongTien);
                }
            }

            // TransactionBehavior will automatically commit if no exception is thrown, otherwise it will rollback

            return Result.Success(true);
        }
    }

}
