using MediatR;
using Microsoft.Extensions.Options;
using Application.Models;
using Application.OptionPatternModel;
using Domain;
using Infrastructure.Exceptions;
using Application.Refits;
namespace Application.CQRS
{
    public class GetUserSystemTokenQuery : IRequest<string>
    {
    }

    public class GetUserSystemTokenQueryHandler : IRequestHandler<GetUserSystemTokenQuery, string>
    {
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;

        public GetUserSystemTokenQueryHandler(IOptions<AppSettingsOption> options, ISSOClient sSOClient)
        {
            _appSettingsOption = options.Value;
            _sSOClient = sSOClient;
        }

        public async Task<string> Handle(GetUserSystemTokenQuery request, CancellationToken cancellationToken)
        {
            var responseUserSystem = await _sSOClient.Authenticate(new GetUserSystemTokenModel
            {
                Username = _appSettingsOption.Settings.UserSystem.UserName,
                Password = _appSettingsOption.Settings.UserSystem.Password
            });
            if (responseUserSystem.IsSuccessStatusCode is false)
                throw new AppException(Messages.AnErrorHasOccurred);

            string token = null;
            if (responseUserSystem.Headers.TryGetValues(ConstantValues.SetCookie, out IEnumerable<string> values))
                token = values.FirstOrDefault()?.Split(';').FirstOrDefault();

            if (string.IsNullOrWhiteSpace(token))
                throw new AppException(Messages.DataNotFound);

            return token;
        }
    }
}
