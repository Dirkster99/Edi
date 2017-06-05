namespace MiniUML.Model.ViewModels.Document
{
  using MiniUML.Framework;

  public class ExportDocumentWindowViewModel : BaseViewModel
  {
    #region fields
    private double mResolution;
    private bool mTransparentBackground;
    private bool mEnableTransparentBackground;
    #endregion fields

    #region properties
    public double prop_Resolution
    {
      get
      {
        return this.mResolution;
      }

      set
      {
        this.mResolution = value;
        this.NotifyPropertyChanged(() => this.prop_Resolution);
      }
    }

    public bool prop_TransparentBackground
    {
      get
      {
        return this.mTransparentBackground;
      }

      set
      {
        this.mTransparentBackground = value;
        this.NotifyPropertyChanged(() => this.prop_TransparentBackground);
      }
    }

    public bool prop_EnableTransparentBackground
    {
      get
      {
        return this.mEnableTransparentBackground;
      }

      set
      {
        this.mEnableTransparentBackground = value;
        this.NotifyPropertyChanged(() => this.prop_EnableTransparentBackground);
      }
    }
    #endregion properties
  }
}
