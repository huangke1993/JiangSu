namespace JiangSuPad.Config.ConfigModel
{
    internal class Configs
    {
        public string DeviceMac { get; set; }
        public DeviceType DeviceType { get; set; }
        public string PadUrl { get; set; }
        public string TvUrl { get; set; }
    }

    internal enum DeviceType
    {
        Ts=0,
        Dk=1,
        Hs=2,
        Hx=3
    }
}
