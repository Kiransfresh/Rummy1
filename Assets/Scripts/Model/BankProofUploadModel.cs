using System;

[Serializable]
public class BankProofUploadModel
{
    public string path;
    public string full_path;
}

[Serializable]
public class BankDetailsModel
{
    public string account_number;
    public string account_holder_name;
    public string ifsc_code;
    public string bank_name;
    public string branch_name;
    public string bank_proof;
    
}
