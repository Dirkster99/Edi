namespace MiniUML.Model.ViewModels.Document
{
  using Framework;

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
        return mResolution;
      }

      set
      {
        mResolution = value;
        NotifyPropertyChanged(() => prop_Resolution);
      }
    }

    public bool prop_TransparentBackground
    {
      get
      {
        return mTransparentBackground;
      }

      set
      {
        mTransparentBackground = value;
        NotifyPropertyChanged(() => prop_TransparentBackground);
      }
    }

    public bool prop_EnableTransparentBackground
    {
      get
      {
        return mEnableTransparentBackground;
      }

      set
      {
        mEnableTransparentBackground = value;
        NotifyPropertyChanged(() => prop_EnableTransparentBackground);
      }
    }
    #endregion properties
  }
}
