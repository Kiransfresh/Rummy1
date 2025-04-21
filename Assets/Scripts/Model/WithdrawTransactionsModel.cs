using System;

[Serializable]
public class WithdrawTransactionsModel
{
    public string id;
    public string withdrawal_id;
    public string players_id;
    public string transfer_type;
    public string transfer_to;
    public string paytm_mobile_number;
    public string paytm_person_name;
    public string account_number;
    public string account_holder_name;
    public string ifsc_code;
    public string bank_name;
    public string branch_name;
    public string request_amount;
    public string tds_percentage;
    public string tds_charge;
    public string service_charge_percentage;
    public string service_charge;
    public string net_receivable;
    public string request_status;
    public string request_date_time;
    public string response_date_time;
    public string status;
    public string transfer_utr_ref_id;
}
