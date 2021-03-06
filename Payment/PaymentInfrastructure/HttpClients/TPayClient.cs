using Flaminco.CommitResult;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PaymentDomain.Features.TPay.DTO.Command;
using PaymentEntities.Entities;
using SharedModule.Extensions;
using System.Net;
using System.Net.Http.Json;

namespace PaymentInfrastructure.HttpClients;

public class TPayClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TPayClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _httpClient.BaseAddress = new Uri(configuration["PaymentSettings:TPay:baseUrl"]);
    }

    public async Task<ICommitResult<TPayInitializerResponse>> InitializerAsync(TPayInitializerRequest request, Product product, CancellationToken cancellationToken)
    {
        string _orderInfo = string.Format("{0}:{1}", DateTime.UtcNow.ToString("yyyyMMddHH:mm:ss"), _httpContextAccessor.GetIdentityUserId());

        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("api/TPay.svc/json/InitializePremiumDirectPaymentTransaction", new TPayEndpointInitializerRequest
        {
            OrderInfo = _orderInfo,
            Language = request.Language,
            MSISDN = request.MSISDN,
            OperatorCode = request.OperatorCode,
            ProductName = product.Name,
            ProductPrice = (double)product.Price,
            Signature = GenerateInitializerSignature(request, product, _orderInfo)
        }, cancellationToken: cancellationToken);

        if (httpResponse.StatusCode == HttpStatusCode.OK)
        {
            return ResultType.Ok.GetValueCommitResult(await httpResponse.Content.ReadFromJsonAsync<TPayInitializerResponse>(cancellationToken: cancellationToken));
        }
        else
        {
            return ResultType.Invalid.GetValueCommitResult<TPayInitializerResponse>(default, "X0000", httpResponse.ReasonPhrase);
        }
    }

    public async Task<ICommitResult<TPayEndpointConfirmPaymentResponse>> ConfirmPaymentAsync(string PinCode, string TransactionId, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("api/TPAY.svc/Json/ConfirmDirectPaymentTransaction", new TPayEndpointConfirmPaymentRequest
        {
            PinCode = PinCode,
            TransactionId = TransactionId,
            Signature = GenerateConfirmationSignature(TransactionId, PinCode)
        }, cancellationToken: cancellationToken);

        if (httpResponse.StatusCode == HttpStatusCode.OK)
        {
            return ResultType.Ok.GetValueCommitResult(await httpResponse.Content.ReadFromJsonAsync<TPayEndpointConfirmPaymentResponse>(cancellationToken: cancellationToken));
        }
        else
        {
            return ResultType.Invalid.GetValueCommitResult<TPayEndpointConfirmPaymentResponse>(default, "X0000", httpResponse.ReasonPhrase);
        }

    }

    public async Task<ICommitResult<TPayEndpointResendPinCodeResponse>> ResendPinCodeAsync(string TransactionId, int Language, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("api/TPAY.svc/Json/ResendVerificationPin", new TPayEndpointResendPinCodeRequest
        {
            TransactionId = TransactionId,
            Signature = GenerateResendPinCodeSignature(TransactionId, Language)
        }, cancellationToken: cancellationToken);

        if (httpResponse.StatusCode == HttpStatusCode.OK)
        {
            return ResultType.Ok.GetValueCommitResult(await httpResponse.Content.ReadFromJsonAsync<TPayEndpointResendPinCodeResponse>(cancellationToken: cancellationToken));
        }
        else
        {
            return ResultType.Invalid.GetValueCommitResult<TPayEndpointResendPinCodeResponse>(default, "X0000", httpResponse.ReasonPhrase);
        }

    }


    private string GenerateInitializerSignature(TPayInitializerRequest content, Product product, string OrderInfo)
    {
        string contentToHash = string.Format("{0}{1}{2}{3}{4}{5}", product.Name, (double)product.Price, content.MSISDN, content.OperatorCode, OrderInfo, content.Language);
        var hash = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(_configuration["PaymentSettings:TPay:PrivateKey"]));
        var correctHash = string.Join(string.Empty, hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(contentToHash)).Select(b => b.ToString("x2")));
        return _configuration["PaymentSettings:TPay:PublicKey"] + ":" + correctHash;
    }
    private string GenerateConfirmationSignature(string TransactionId, string PinCode)
    {
        string contentToHash = string.Format("{0}{1}", TransactionId, PinCode);
        var hash = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(_configuration["PaymentSettings:TPay:PrivateKey"]));
        var correctHash = string.Join(string.Empty, hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(contentToHash)).Select(b => b.ToString("x2")));
        return _configuration["PaymentSettings:TPay:PublicKey"] + ":" + correctHash;
    }
    private string GenerateResendPinCodeSignature(string TransactionId, int Language)
    {
        string contentToHash = string.Format("{0}{1}", TransactionId, Language);
        var hash = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(_configuration["PaymentSettings:TPay:PrivateKey"]));
        var correctHash = string.Join(string.Empty, hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(contentToHash)).Select(b => b.ToString("x2")));
        return _configuration["PaymentSettings:TPay:PublicKey"] + ":" + correctHash;
    }
}
