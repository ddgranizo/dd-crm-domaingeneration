   M S B A M L   `   `   `  ���� B  >ModelUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:ModelUI.ViewsModelUI.Views  7 clr-namespace:ModelUI.ViewmodelsModelUI.Viewmodels  7 clr-namespace:ModelUI.ConvertersModelUI.Converters  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem !    ModelUI.Views.DomainManager   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:ModelUI.Views     /vm clr-namespace:ModelUI.Viewmodels     1conv clr-namespace:ModelUI.Converters     3fa$http://schemas.fontawesome.io/icons/     6sys&clr-namespace:System;assembly=mscorlib     V RMaterialDesignThemes.Wpf, Version=2.5.1.1345, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes     MaterialDesignBody#���   $��Regular"�$��13(�&   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�  MaterialDesignPaper#���    MaterialDesignFont#���   � Title$ Domain Manager��$	��450��$	��800��.��� -    �Name$ 
LayoutRoot��
������� � 	Resources /   )ModelUI.Viewmodels.DomainManagerViewmodel  	viewModel2   ,ModelUI.Converters.BoolToVisibilityConverter  BoolToVisibilityConverter��%   &	       &	          .+��  � DataContext ��  �� Source# [ .+�� $��1�� � RowDefinitions ��  � ColumnDefinitions �� $��* ��� $
��auto ��� $��2* �.+�� $��1�� �� $
��auto ��� $��* �.+��� $��1��$��Dominios��.���  �� $
��auto ��� $��* �.+�� $��0�� �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $��* ��� $
��auto ��� $
��auto �.+��� $��0��$��5q�$��30��$��30�� + 'MaterialDesignFloatingActionLightButton#���  	 � ToolTip$	 Añadir nueva entidad������ # [ 
 �� Path$ 
 AddNewDomainRequestCommand� �� Mode$ OneWay=�.��'  !MaterialDesignThemes.Wpf.PackIcon  �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=2.5.1.1345, Culture=neutral, PublicKeyToken=null]]   Kind$	 Add=�$��20��$��20���� $��1��$��5q�$��30��$��30��#���  $#	 Eliminar entidad seleccionada������ # [ $ 
 DeleteDomainRequestCommand�$ OneWay=�.��  $ Delete=�$��20��$��20���� -    ��Name$ DomainsScrollViewer��$��1��$��5q�.���� -    ��Name$ DomainsList������ *DomainsCollection+ �� SelectedItem �� *SelectedDomain+���� .R��  �� $
��auto ��� $��* ��� $
��auto �.+��� $��2��$��Center=����� *
DomainName+�� $��Right=�$��Stretch=�$��1�� �� ResizeBehavior$ PreviousAndNext=�$��5��
�������� $��2��.+��� $��Editar Dominio��.��� � $��2��y��� *IsAddingNewDomain+ �� 	Converter# [ .+��� $��Nuevo dominio��.���  �� $
��auto ��� $
��auto ��� $��* � �� $
��auto ��� $��* �.+��� $��0��$��0��$��5q�$��Center=�Nombre de dominio:�� $��1��$��0������ *NewDomainName+ �� UpdateSourceTrigger$ PropertyChanged=��� $��0��$��1��$��5q�$��Center=�Path:�� $��1��$��1�� �� 
IsReadOnly . ���� *NewDomainPath+$ OneWay=��� $��2��$��Bottom=�$��1��$��Right=� �� Orientation$ 
Horizontal=�.+��� $��5q�$��30��$��30��#���  $	 Cancelar cambios������ # [ $
 CancelRequestCommand�$ OneWay=�.��  $ Cancel=�$��20��$��20���� $��5q�$��30��$��30��#���  $	 Guardar cambios������ # [ $$
 ConfirmNewDomainRequestCommand�$ OneWay=�.��  $	 Add=�$��20��$��20��