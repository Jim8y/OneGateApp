using Neo;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Native;
using NeoOrder.OneGate.Models.Intents;
using NeoOrder.OneGate.Services.RPC;
using System.Numerics;
using System.Text.Json.Nodes;

namespace NeoOrder.OneGate.Pages;

public partial class SendingPage : ContentPage, IQueryAttributable
{
    readonly CancellationTokenSource cancellation = new();
    readonly RpcClient rpcClient;

    public required Transaction Transaction { get; set { field = value; OnPropertyChanged(null); } }
    public required TransactionIntent[] Intents { get; set { field = value; OnPropertyChanged(); } }
    public ulong? BlockTime { get; set { field = value; OnPropertyChanged(); } }

    public long Fee => (Transaction?.SystemFee + Transaction?.NetworkFee) ?? 0;
    public BigDecimal DecimalFee => new((BigInteger)Fee, NativeContract.GAS.Decimals);
    public string DisplayFee => $"{DecimalFee} {NativeContract.GAS.Symbol}";
    public BigDecimal DecimalSystemFee => new((BigInteger)(Transaction?.SystemFee ?? 0), NativeContract.GAS.Decimals);
    public BigDecimal DecimalNetworkFee => new((BigInteger)(Transaction?.NetworkFee ?? 0), NativeContract.GAS.Decimals);
    public string FeeDetails => $"{DecimalSystemFee} (sys) + {DecimalNetworkFee} (net)";

    public SendingPage(RpcClient rpcClient)
    {
        this.rpcClient = rpcClient;
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Transaction = (Transaction)query["tx"];
        Intents = (TransactionIntent[])query["intents"];
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        QueryTransactionStatus();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await cancellation.CancelAsync();
        cancellation.Dispose();
    }

    async void QueryTransactionStatus()
    {
        try
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(15), cancellation.Token);
                JsonObject tx;
                try
                {
                    tx = await rpcClient.RpcSendAsync<JsonObject>("getrawtransaction", Transaction.Hash, true);
                }
                catch (RpcException)
                {
                    continue;
                }
                BlockTime = tx["blocktime"]?.GetValue<ulong>();
                if (BlockTime.HasValue) break;
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
