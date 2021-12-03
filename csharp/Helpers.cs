public class Helpers {
    public static string PadDay(string day) {
        return day.Length < 2 ? $"0{day}" : day;
    }

    public static uint ParseBinary(string s) {
        return Convert.ToUInt32(s, 2);
    }
}