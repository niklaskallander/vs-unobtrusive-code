namespace UnobtrusiveCode.Options
{
    using Community.VisualStudio.Toolkit;

    using System.Runtime.InteropServices;

    public static class DialogPageProvider
    {
        [ComVisible(true)]
        internal class General
            : BaseOptionPage<UnobtrusiveCodeOptionsRaw>
        {
        }
    }
}
