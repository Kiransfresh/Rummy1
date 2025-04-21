using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public class SERVER_DETAILS
    {
        /* Dev Server                                       
        public const string Host = "devgame.rummycirclez.com";
        public const string API = "https://dev.rummycirclez.com/api";
        public const string Mobile_Url = "https://dev.rummycirclez.com/";
        public const string APK_LINK = "https://dev.rummycirclez.com/apk/Colours-Rummy.apk";// */


        //* Live Server                                       
        public const string Host = "livegame.sprrummee.com";
        public const string API = "https://www.sprrummee.com/api";
        public const string Mobile_Url = "https://www.sprrummee.com/";
        public const string APK_LINK = "https://www.sprrummee.com/apk/RummyCirclez.apk";// */

        public const int TcpPort = 9933;
        public const int WsPort = 8080;
        public const int WssPort = 8443;
        public const string Zone = "GameZone";
    }

    public class Country {
        public const string currency_symbol = "₹";
        public const string currency_code = "INR";
    }

    public class GAME_CONFIG
    {
        public const bool bonus = true;
    }

    public class CONFIG
    {
        public static bool is_paid = true;
    }

    public class SCENE_NAMES
    {
        public const string Game_Lobby = "GameLobby";
        public const string Game_Room = "gameRoom";
    }

    public class KEYS
    {
        public const string unique_name = "unique_name";
        public const string status = "status";
        public const string parameter = "params";
        public const string cmd = "cmd";
        public const string valid = "valid";
        public const string invalid = "invalid";
        public const string packet = "packet";
        public const string id = "id";
        public const string game_id = "game_id";
        public const string table_id = "table_id";
        public const string mobile_number = "mobile_number";
        public const string otp_code = "otp_code";
        public const string requesting_source = "Requesting-Source";
        public const string auth_token = "auth_token";
        public const string wallet = "wallet";
        public const string table_event_type = "table_event_type";
        public const string is_splash_shown = "is_splash_shown";
        public const string discarded_card = "discarded_card";
        public const string is_declare = "is_declare";
        public const string is_close_deck = "is_close_deck";
        public const string card_group = "card_group";
        public const string Selection_Counter = "Selection_Counter";
        public const string is_turbo = "is_turbo";
        public const string is_private_table = "is_private_table";
        public const string chips = "chips";
        public const string coins = "coins";
        public const string type_of_issue = "type_of_issue";
        public const string message = "message";
        public const string version_code = "version_code";
        public const string device_type = "device_type";
        public const string name = "name";
        public const string email = "email";
        public const string mobile = "mobile";
        public const string whats_app_number = "whats_app_number";
        public const string how_soon_required = "how_soon_required";
        public const string budget = "budget";
        public const string type_of_solutions = "type_of_solutions";
        public const string captcha = "captcha";
        public const string new_email = "new_email";
        public const string first_name = "first_name";
        public const string last_name = "last_name";
        public const string gender = "gender";
        public const string date_of_birth = "date_of_birth";
        public const string avatar_path = "avatar_path";  //coins
        public const string display_in = "display_in";
        public const string amount = "amount";
        public const string bonus_code = "bonus_code";
        public const string username = "username";
        public const string current_password = "current_password";
        public const string new_password = "new_password";
        public const string confirm_new_password = "confirm_new_password";
        public const string aadhar = "aadhar";
        public const string pan = "pan";
        public const string account_number = "account_number";
        public const string account_holder_name = "account_holder_name";
        public const string ifsc_code = "ifsc_code";
        public const string bank_name = "bank_name";
        public const string bank_proof = "bank_proof";
        public const string refer_code = "refer_code";
        public const string branch_name = "branch_name";
        public const string file = "file";
        public const string is_accept = "is_accept";
        public const string Block = "Block";
        public const string tournament_id = "tournament_id";
        public const string aadhar_front_image = "aadhar_front_image";
        public const string aadhar_back_image = "aadhar_back_image";
        public const string pan_card_image = "pan_card_image";
        public const string FCMToken = "fcm_token";
        public const string terms_and_privacy_policy = "terms_and_privacy_policy";
        public const string address = "address";
    }

    public class REQUEST
    {
        public const string GAMES = "GAMES";
        public const string JOIN_TABLE = "JOIN_TABLE";

        public const string CREATE_PRIVATE_TABLE = "CREATE_PRIVATE_TABLE";
        public const string JOIN_PRIVATE_TABLE = "JOIN_PRIVATE_TABLE";
        public const string COINS_VALUE = "COINS_VALUE";
        public const string CONFIG = "CONFIG";

        public const string GAME_PLAY_EVENT = "GAME_PLAY_EVENT";
        public const string USER_INFORMATION = "USER_INFORMATION";
        public const string UPDATE_CHIPS = "UPDATE_CHIPS";
        public const string UPDATE_COINS = "UPDATE_COINS";
        public const string CLIENT_PINGING = "CLIENT_PINGING";
        public const string SPLIT = "SPLIT";
        public const string REJOIN_GAME = "REJOIN_GAME";
        public const string ON_GOING_GAME_LIST = "ON_GOING_GAME_LIST";
        public const string LAST_ROUND_RESULT = "LAST_ROUND_RESULT";
        public const string SCORE_BOARD = "SCORE_BOARD";
        public const string LIVE_PLAYERS = "LIVE_PLAYERS";
        public const string REGISTERED_PLAYERS = "REGISTERED_PLAYERS";      

        public const string TOURNAMENT_LIST = "TOURNAMENT_LIST";
        public const string TOURNAMENT_JOINED_PLAYERS_COUNT = "TOURNAMENT_JOINED_PLAYERS_COUNT";
        public const string TOURNAMENT_DATA = "TOURNAMENT_DATA";
    }

    public class GAME_TABLE_EVENT
    {
        public const string PLAYER_TAKE_SEAT = "PLAYER_TAKE_SEAT";
        public const string PLAYER_LEAVE_TABLE = "PLAYER_LEAVE_TABLE";
        public const string PLAYER_SWITCH_TABLE = "PLAYER_SWITCH_TABLE";
        public const string PLAYER_DISCONNECTED = "PLAYER_DISCONNECTED";
        public const string PLAYER_REMOVED = "PLAYER_REMOVED";
        public const string WAITING_FOR_OTHER_PLAYER = "WAITING_FOR_OTHER_PLAYER";
        public const string WAITING_FOR_GAME_START = "WAITING_FOR_GAME_START";
        public const string TOSS = "TOSS";
        public const string RE_ARRANGE = "RE_ARRANGE";
        public const string CARD_DEALING = "CARD_DEALING";
        public const string PLAYER_DROP = "PLAYER_DROP";
        public const string PLAYER_EXTRA_TIME = "PLAYER_EXTRA_TIME";
        public const string PLAYER_AUTO_PLAY = "PLAYER_AUTO_PLAY";
        public const string REQUEST_MELD_CARD = "REQUEST_MELD_CARD";
        public const string CARD_PICKED = "CARD_PICKED";
        public const string PLAYER_TURN_FINISH = "PLAYER_TURN_FINISH";
        public const string RECEIVED_MELD_CARD = "RECEIVED_MELD_CARD";
        public const string GROUP_HAND_CARDS = "GROUP_HAND_CARDS";
        public const string TOGGLE_AUTO_PLAY = "TOGGLE_AUTO_PLAY";
        public const string GAME_PLAY_RESULT = "GAME_PLAY_RESULT";
        public const string GAME_END = "GAME_END";
        public const string INFO_MESSAGE = "INFO_MESSAGE";
        public const string EXISTING_PLAYER_JOIN_TABLE = "EXISTING_PLAYER_JOIN_TABLE";
        public const string UPDATE_RESULT_TIMER = "UPDATE_RESULT_TIMER";
        public const string UPDATE_AUTO_PLAY = "UPDATE_AUTO_PLAY";
        public const string UPDATE_DECK_CARDS = "UPDATE_DECK_CARDS";
        public const string REFRESH_TABLE = "REFRESH_TABLE";
    }

    public class MESSAGE
    {
        public const string CONNECTING = "Connecting";
        public const string WAITING_FOR_PLAYER = "Waiting for other players";
        public const string WAITING_FOR_GAME = "Game will start in ";
        public const string LEAVE_TABLE_CONFIRMATION = "If you leave the table now, you will be dropped from the current game. The remaining amount will be credited back to your account.";
        public const string LEAVE_TABLE_CONFIRMATION_PRACTICE = "Are you sure, You want to leave the table?";
        public const string DROP_CONFIRMATION = "Are you sure, you want to drop?";
        public const string INVALID_TURN = "Please wait for your turn";
        public const string MELD_CONFIRMATION = "Highlighted groups have been auto melded. Do you want to re-check?";
        public const string SHOW_CONFIRMATION = "Are you sure you want to place the show?";
        public const string AUTO_DROP_CONFIRMATION = "Are you sure, you want to drop? You will drop on your next turn";

        public const string BANK_PRROF_WARNING = "Sorry! Please upload proof to perform this action. Right now there is no proof exist into system releated to your bank details";

        public const string ENTER_ACCOUNT_HOLDER = "Please enter account holder name for authorization purpose";
        public const string ENTER_BANK_NAME = "Please enter bank name for authorization purpose";
        public const string ENTER_ACCOUNT_NUMBER = "Please enter account number. It is mandatory";
        public const string ENTER_IFSC = "Please enter ifsc code for authorization purpose";
        public const string ENTER_BRANCH_NAME = "Please enter branch name for authorization purpose";

        public const string ENTER_VALID_EMAIL = "Please enter a valid email address to proceed with the profile update";
        public const string ENTER_VALID_INFO = "Please enter a valid information to proceed with the profile update";

        public const string WITHDRAW_WARNING = "Are you sure do you want to proceed with the withdrawal request?";
        public const string WITHDRAW_WARNING_V2 = "Please correct your requested amount. Your requested amount must be less or equal to your withdrawal amount";
        public const string WITHDRAW_AMOUNT_WARNING = "The minimum withdrawal amount must be at least " + Country.currency_symbol +  "100.00";
        public const string TRANSACTION_ERROR = "Sorry! There is no transaction found in your wallet history";
        public const string CONTACT_US_ERROR = "Please enter message, So that our support team can get back to you!";
        public const string Location_Access_Message = "Allow access to the Location, This helps us ensure that you’re playing Colours Rummy from a state where Online Rummy is not restricted. We do not use it for any other purposes.";
        public const string Storage_Access_Message = "Allow access to Photos, Media, and Files, Access to these resources is essential to facilitate the KYC verification process. This information is used to verify your identity, ensuring that you have a safe and enjoyable gaming experience.";
    }

    public class API_METHODS
    {
        public const string LOGIN_WITH_MOBILE = SERVER_DETAILS.API + "/login/with_mobile"; // used in free game for direct registration
        public const string LOGIN = SERVER_DETAILS.API + "/login";
        public const string EMAIL_LOGIN = SERVER_DETAILS.API + "/email_login";
        public const string REGISTER = SERVER_DETAILS.API + "/register";
        public const string VERIFY_OTP = SERVER_DETAILS.API + "/login/verify_mobile_number"; // used in free game for mobile verifcation
        public const string UPDATE_USERNAME = SERVER_DETAILS.API + "/profile/update_username";
        public const string GUEST_LOGIN = SERVER_DETAILS.API + "/create_guest_login";
        public const string REPORT_A_PROBLEM_REASON = SERVER_DETAILS.API + "/report_a_problem/reasons";
        public const string REPORT_A_PROBLEM = SERVER_DETAILS.API + "/report_a_problem ";

        public const string ENQUIRY_FORM = SERVER_DETAILS.API + "/submit_get_quote_request ";

        public const string TERMS_CONDITIONS = SERVER_DETAILS.API + "/cms_pages/terms_and_conditions";
        public const string PRIVATE_POLICY = SERVER_DETAILS.API + "/cms_pages/privacy_policy";
        public const string ABOUT_US = SERVER_DETAILS.API + "/cms_pages/about_us";
        public const string Legality = SERVER_DETAILS.API + "/cms_pages/legality";
        public const string Responsible_Gaming = SERVER_DETAILS.API + "/cms_pages/responsible_gaming";
        public const string PROFILE = SERVER_DETAILS.API + "/profile";
        public const string UPDATE_PROFILE = SERVER_DETAILS.API+ "/update_profile";
        public const string UPDATE_EMAIL = SERVER_DETAILS.API + "/update_profile/email";
        public const string DEFAULT_AVATARS = SERVER_DETAILS.API + "/avatars";
        public const string UPDATE_AVATAR = SERVER_DETAILS.API + "/update_profile/avatar";
        public const string CHECK_VERSION = SERVER_DETAILS.API + "/check_for_version_update?current_version=";
        public const string ADS_IMAGES = SERVER_DETAILS.API + "/ads";

        public const string FETCH_NOTIFICATIONS = SERVER_DETAILS.API + "/notifications";
        public const string NOTIFICATION_MARK_READ = SERVER_DETAILS.API + "/notifications/mark_as_read";

        public const string REFER_FRIEND_MESSAGE = SERVER_DETAILS.API + "/refer_friend/info";
        
        public const string DEPOSIT = SERVER_DETAILS.API + "/deposite";
        public const string ADD_COINS = SERVER_DETAILS.API + "/add_coins";
        public const string DEPOSIT_VALIDATION = SERVER_DETAILS.API + "/deposite_validation";

        public const string FORGOT_PASSWORD = SERVER_DETAILS.API + "/forgot_password";
        public const string CHANGE_PASSWORD = SERVER_DETAILS.API + "/change_password";

        public const string ADD_KYC = SERVER_DETAILS.API + "/kyc";
        public const string FETCH_KYC_DETAILS = SERVER_DETAILS.API + "/kyc_details";

        public const string UPLOAD_BANK_PROOF = SERVER_DETAILS.API + "/bank_account_proof_upload";
        public const string ADD_BANK_DETAILS = SERVER_DETAILS.API + "/bank_details";
        public const string FETCH_BANK_DETAILS = SERVER_DETAILS.API + "/get_bank_details";
        public const string CONTACT = SERVER_DETAILS.API + "/contact";

        public const string VERIFY_WITHDRAW = SERVER_DETAILS.API + "/verfie_withdraw";
        public const string WITHDRAW_REQUEST = SERVER_DETAILS.API + "/withdraw_request";

        public const string DEPOSIT_LIST = SERVER_DETAILS.API + "/deposit_list";
        public const string WITHDRAW_LIST = SERVER_DETAILS.API + "/withdraw_list";
        public const string BONUS_LIST = SERVER_DETAILS.API + "/bonus_list";
        public const string GAME_HISTORY = SERVER_DETAILS.API + "/game_history";


        public const string EMAIL_REFER = SERVER_DETAILS.API + "/email_refer";
    
        public const string SEND_OTP = SERVER_DETAILS.API + "/login/send_verification_otp";
        public const string OTP_VERIFICATION = SERVER_DETAILS.API + "/login/verification_otp";
        public const string TOURNAMENT_REGISTER = SERVER_DETAILS.API + "/tournament_register";

        public const string HOSTED_PRIVATE_GAMES_DETAILS = SERVER_DETAILS.API + "/hosted_private_games_details";
        public const string CHECK_LOCATION = SERVER_DETAILS.API + "/check_location";
    }

    public class URL
    {
        public const string HOW_TO_PLAY = SERVER_DETAILS.Mobile_Url + "/help/mobile";
        public const string ABOUT_US = SERVER_DETAILS.Mobile_Url + "/mobile/about_us";
        public const string TERMS_AND_CONDITIONS = SERVER_DETAILS.Mobile_Url + "mobile/terms_and_conditions";
        public const string PRIVACY_POLICY = SERVER_DETAILS.Mobile_Url + "/mobile/privacy_policy";
        public const string LEGALITY = SERVER_DETAILS.Mobile_Url + "/mobile/legality";
        public const string RESPONSIBLE_GAMING = SERVER_DETAILS.Mobile_Url + "/mobile/responsible_gaming";
    }

    public class DEVICE_TYPE
    {
        public const string Unity_android = "Unity_android";
        public const string Unity_Ios = "Unity_Ios";
        public const string Unity_Webgl = "Unity_Webgl";
    }
    
    public class GAME_TYPE
    {
        public const string CASH = "Cash";
        public const string PRACTICE = "Practice";
    }
    
    public class GAME_SUB_TYPE
    {
        public const string POINT = "Points";
        public const string POOL = "Pool";
        public const string DEALS = "Deals";
    }

    public class POOL_TYPE
    {
        public const int POOL_101 = 101;
        public const int POOL_201 = 201;
    }

    public class SEAT
    {
        public const int PLAYER_2 = 2;
        public const int PLAYER_4 = 4;
        public const int PLAYER_6 = 6;
    }
    

    public class TURN_ENUM
    {
        public const string NONE = "NONE";
        public const string PLAYER_TURN_INITIAL_TIMER = "PLAYER_TURN_INITIAL_TIMER";
        public const string PLAYER_TURN_EXTRA_TIME = "PLAYER_TURN_EXTRA_TIME";
        public const string PLAYER_TURN_AUTO_PLAY_CONFIRMATION_TIME = "PLAYER_TURN_AUTO_PLAY_CONFIRMATION_TIME";
        public const string PLAYER_DECLARE_TIMER = "PLAYER_DECLARE_TIMER";
    }

    public class PLAYER_ENUM
    {
        public const string NONE = "NONE";
        public const string MIDDLE_DROP = "MIDDLE_DROP";
        public const string WINNER = "WINNER";
        public const string WRONG_SHOW = "WRONG_SHOW";
        public const string DROP = "DROP";
    }

    public class GAME_ENUM
    {
        public const string NONE = "NONE";
        public const string TOSS = "TOSS";
        public const string RE_ARRANGE = "RE_ARRANGE";
        public const string GAME_DEALING = "GAME_DEALING";
        public const string PLAYER_TURN = "PLAYER_TURN";
        public const string GAME_DECLARE = "GAME_DECLARE";
        public const string GAME_RESULT = "GAME_RESULT";
        public const string GAME_END = "GAME_END";
    } 
    
    public class TABLE_ENUM
    {
        public const string NONE = "NONE";
        public const string WAITING_FOR_PLAYER = "WAITING_FOR_PLAYER";
        public const string WAITING_FOR_GAME = "WAITING_FOR_GAME";
        public const string GAME_RUNNING = "GAME_RUNNING";
        public const string END_TABLE = "END_TABLE";
    }

    public class CARD_SUIT
    {
        public const string HEART = "HEART";
        public const string CLUB = "CLUB";
        public const string DIAMOND = "DIAMOND";
        public const string SPADE = "SPADE";
        public const string JOKER = "JOKER";
    }

    public class CARD_SPACE
    {
        public const int WEBGL_SLOT = 90;
        public const int WEBGL_CARD = 35;
        public const int OTHER_SLOT = 100;
        public const int OTHER_CARD  = 25;
    }

    public class TOURNAMENT_STATUS 
    {
        public const string CREATED = "CREATED";
        public const string REGISTRATION_START = "REGISTRATION_START";
        public const string REGISTRATION_CLOSE = "REGISTRATION_CLOSE";
        public const string RUNNING = "RUNNING";
        public const string COMPLETED = "COMPLETED";
        public const string REJECTED = "REJECTED";
        public const string CALL_OFF = "CALL_OFF";
    }

    public class PLAYER_PREFS_CONSTANTS
    {
        public const string AVATARIMAGE_ID = "Avatar_Image_Id";
    }

}
