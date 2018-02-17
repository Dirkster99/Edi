namespace MiniUML.Plugins.UmlClassDiagram.Controls.View.Connect
{
  using System;
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;
  using Model.ViewModels.Shapes;
  using MiniUML.View.Controls;
  using MiniUML.View.Converter;

  /// <summary>
  /// Interaction logic for Association.xaml
  /// </summary>
  public partial class GenericUmlAssociation : UserControl, INotifyPropertyChanged, ISnapTarget
  {
    #region fields
    private static readonly DependencyProperty FromNameProperty = DependencyProperty.Register(
        "FromName",
        typeof(string),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

    private static readonly DependencyProperty ToNameProperty = DependencyProperty.Register(
        "ToName",
        typeof(string),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

    private static readonly DependencyProperty FromArrowProperty = DependencyProperty.Register(
        "FromArrow",
        typeof(string),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

    private static readonly DependencyProperty ToArrowProperty = DependencyProperty.Register(
        "ToArrow",
        typeof(string),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

    private static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
        "Stroke", typeof(Brush), typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    private ShapeIdToControlConverter shapeIdConverter;
    #endregion fields

    #region constructor
    /// <summary>
    /// static class constructor
    /// </summary>
    static GenericUmlAssociation()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(GenericUmlAssociation),
      new FrameworkPropertyMetadata(typeof(GenericUmlAssociation)));
    }

      #endregion constructor

    #region events
    /// <summary>
    /// This event is part of the <seealso cref="ISnapTarget"/> Interface.
    /// </summary>
    public event SnapTargetUpdateHandler SnapTargetUpdate;

    /// <summary>
    /// Standard event of the <seealso cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion events

    #region Dependency properties (FromName, FromArrow, ToName, ToArrow)
    public string FromName
    {
      get => (string)GetValue(FromNameProperty);
        set => SetValue(FromNameProperty, value);
    }

    public string ToName
    {
      get => (string)GetValue(ToNameProperty);
        set => SetValue(ToNameProperty, value);
    }

    public string FromArrow
    {
      get => (string)GetValue(FromArrowProperty);
        set => SetValue(FromArrowProperty, value);
    }

    public string ToArrow
    {
      get => (string)GetValue(ToArrowProperty);
        set => SetValue(ToArrowProperty, value);
    }

    public Brush Stroke
    {
      get => (Brush)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public ShapeIdToControlConverter ShapeIdToControlConverter
    {
      get
      {
        if (shapeIdConverter == null)
        {
                    shapeIdConverter = new ShapeIdToControlConverter
                    {
                        ReferenceControl = this
                    };
                }

        return shapeIdConverter;
      }
    }
    #endregion

    #region methods

    public void SnapPoint(ref Point snapPosition, out double snapAngle)
    {
      snapAngle = 90;
      ////tpl.SnapPoint(ref snapPosition, out snapAngle);
    }

    public void NotifySnapTargetUpdate(SnapTargetUpdateEventArgs e)
    {
      if (SnapTargetUpdate != null)
        SnapTargetUpdate(this, e);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      ContextMenu = ConnectorViewBase.CreateContextMenu(DataContext as ShapeViewModelBase);
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);

      try
      {
        ShapeIdToControlConverter s = ((ShapeIdToControlConverter)FindResource("ShapeIdToControlConverter"));

        if (s != null)
          s.ReferenceControl = this;
      }
      catch (Exception)
      {
        Console.WriteLine("Exception: ShapeIdToControlConverter not found - target snapping may not work.");
      }
    }

    #region INotifyPropertyChanged implementation

    private void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion

    private void snapTargetUpdate(ISnapTarget source, SnapTargetUpdateEventArgs e)
    {
      NotifySnapTargetUpdate(e);
    }
    #endregion
  }
}
