using System;

[Serializable]
public class DepositTransactionsModel
{
    public string id;
    public string comment;
    public string credits_count;
    public string transaction_id;
    public string payment_gateway_id;
    public string created_at;
    public string updated_at;
    public string transaction_status;
    public string payment_gateway;
}
