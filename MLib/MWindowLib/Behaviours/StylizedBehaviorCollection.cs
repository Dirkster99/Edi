namespace MWindowLib.Behaviours
{
    using System.Windows;
    using System.Windows.Interactivity;

    public class StylizedBehaviorCollection : FreezableCollection<Behavior>
    {
        protected override Freezable CreateInstanceCore()
        {
            return new StylizedBehaviorCollection();
        }
    }
}
