namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaThePhuongTien
{
    public class KhoaThePhuongTienCommandHandler : ICommandHandler<KhoaThePhuongTienCommand, bool>
    {
        private readonly IPhuongTienCommandRepository _phuongTienCommandRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public KhoaThePhuongTienCommandHandler(
            IPhuongTienCommandRepository phuongTienCommandRepository, 
            IDateTimeProvider dateTimeProvider)
        {
            _phuongTienCommandRepository = phuongTienCommandRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<bool>> Handle(KhoaThePhuongTienCommand request, CancellationToken cancellationToken)
        {
            var phuongTiens = await _phuongTienCommandRepository.GetPhuongTiensByTheIdsAsync(request.TheIds, cancellationToken);
            var now = _dateTimeProvider.Now.DateTime;

            if (!phuongTiens.Any())
                return Result.Failure<bool>(PhuongTienErrors.NotFound);

            foreach (var theId in request.TheIds)
            {
                var phuongTien = phuongTiens.FirstOrDefault(x => x.ThePhuongTiens.Any(t => t.Id == theId));
                if (phuongTien != null)
                {
                    phuongTien.KhoaThe(theId, now);
                    _phuongTienCommandRepository.Update(phuongTien);
                }
            }

            // TransactionBehavior will automatically commit if no exception is thrown, otherwise it will rollback

            return Result.Success(true);
        }
    }

}
