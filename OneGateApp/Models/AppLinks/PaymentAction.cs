using Neo;
using Neo.Wallets;
using NeoOrder.OneGate.Pages;
using NeoOrder.OneGate.Services;
using System.Web;

namespace NeoOrder.OneGate.Models.AppLinks;

class PaymentAction : AppLinkAction
{
    protected override string Route => "//wallet/send";
    public UInt160 Account { get; }
    public string Recipient { get; }
    public UInt160? AssetId { get; }
    public decimal? Amount { get; }

    public PaymentAction(Uri uri, ProtocolSettings protocolSettings)
    {
        Account = uri.LocalPath.ToScriptHash(protocolSettings.AddressVersion);
        Recipient = uri.LocalPath;
        var nv = HttpUtility.ParseQueryString(uri.Query);
        if (nv["asset"] is string s_asset)
            AssetId = UInt160.Parse(s_asset);
        if (nv["amount"] is string s_amount)
            Amount = decimal.Parse(s_amount);
    }

    protected override IDictionary<string, object> CreateQuery()
    {
        var query = new Dictionary<string, object>
        {
            ["address"] = Recipient
        };
        if (AssetId is not null) query["asset"] = AssetId;
        if (Amount.HasValue) query["amount"] = Amount.Value;
        return query;
    }

    protected override Page CreatePage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetServiceOrCreateInstance<SendPage>();
    }
}
