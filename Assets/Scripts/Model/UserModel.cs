using System;

[Serializable]
public class UserModel
{
    public string mobile_number;
    public string unique_name;
    public string avatar_path;
    public string auth_token;
    public string player_status;

    public int exisiting_user;
    public Wallet wallet;

    //SAI
    public string gender;
    public string email;
    public string first_name;
    public string last_name;
    public string is_username_updated_on_first_signup;
    public string email_verified;
    public string mobile_verified;
    public string avatar_full_path;
    public string guest_player;
    public string refer_code;

    public string deposite_url;
    public string date_of_birth;

    [Serializable]
    public class Wallet
    {
        public int chips;
        public int in_play_chips;
        public float cash_deposit;
        public float in_play_cash;
        public float total_bonus;
        public int coins;
        public float cash_withdrawal;
        public float cash;
        public string upi_id;
    }
}
