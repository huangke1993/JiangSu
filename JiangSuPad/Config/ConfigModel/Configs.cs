namespace JiangSuPad.Config.ConfigModel
{
    internal class Configs
    {
        public DeviceType DeviceType { get; set; }
        public string PadUrl { get; set; }
        public string TvUrl { get; set; }
    }

    internal enum DeviceType
    {
        Ts=0,
        Dk=1
    }
}
