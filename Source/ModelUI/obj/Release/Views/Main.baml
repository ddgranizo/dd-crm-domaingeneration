   M S B A M L   `   `   `  ���� B  >ModelUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:ModelUI.ViewsModelUI.Views  7 clr-namespace:ModelUI.ViewmodelsModelUI.Viewmodels  7 clr-namespace:ModelUI.ConvertersModelUI.Converters  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem     ModelUI.Views.Main   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:ModelUI.Views     /vm clr-namespace:ModelUI.Viewmodels     1conv clr-namespace:ModelUI.Converters     3fa$http://schemas.fontawesome.io/icons/     6sys&clr-namespace:System;assembly=mscorlib     V RMaterialDesignThemes.Wpf, Version=2.5.1.1345, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes     MaterialDesignBody#���   $��Regular"�$��13(�&   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�  MaterialDesignPaper#���    MaterialDesignFont#���   � Title$
 Main��$	��450��$	��800��.��� -    �Name$ 
LayoutRoot��
������� � 	Resources &    ModelUI.Viewmodels.MainViewmodel  	viewModel2   ,ModelUI.Converters.BoolToVisibilityConverter  BoolToVisibilityConverter��%   &	       &	          .+��  � DataContext ��  �� Source# [ .+��  � RowDefinitions �� $
��auto ��� $��* �.+�)  #MaterialDesignThemes.Wpf.DialogHost  $��2�� K� ZIndex$	 200��	   IsOpen	 �� *IsDialogOpen+
   DialogContent
 � $��20q� �� $
��auto ��� $
��auto �.+��� $��1������ *MessageDialog+]� $��0��.+�o� $	��Top=�
�������.@�m� $��Archivo��.@�m� $��Dominios�� m� Command �� # [  �� Path$ ShowDomainManagerCommand� �� Mode$ OneWay=�� $��1�� ��  � ColumnDefinitions �� $��* ��� $
��auto ��� $��2* �.+�� $��1�� �� $
��auto ��� $��* �.+��� $��1��$��	Entidades��.���  �� $
��auto ��� $
��auto ��� $��* �.+�� $��0�� �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $��* ��� $
��auto ��� $
��auto �.+��� $��0��$��5q�$��30��$��30�� + 'MaterialDesignFloatingActionLightButton#���   � ToolTip$ Añadir nueva entidad������ # [ $ AddEntityRequestCommand�$ OneWay=�.��'  !MaterialDesignThemes.Wpf.PackIcon  �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=2.5.1.1345, Culture=neutral, PublicKeyToken=null]]   Kind$	 Add=�$��20��$��20���� $��1��$��5q�$��30��$��30��#���  $# Eliminar entidad seleccionada������ # [ $ DeleteEntityCommand�$ OneWay=�.��  $ Delete=�$��20��$��20���� $��2��$��5q�$��30��$��30��#���  $3 -Seleccionar/Deseleccionar todas las entidades������ # [ $ SelectUnselectAllCommand�$ OneWay=�.��  $ 	SelectAll=�$��20��$��20����  �� Orientation$ Vertical=�$��3��.+��� #��["��� $ 
Horizontal=�$��4��.+��� $��3��#���  $��30��$��30��$  Editar parámetros comunes������ # [ $" EditCommonDataRequestCommand�$ OneWay=�.��  $ Settings=�$��20��$��20���� $��6��$��Center=�$ 
Horizontal=�.+��� $��Center=�.��%#Entity Definitions and Global enumsd� $��Center=� d� 	IsChecked �� *GenerateGlobalEnums+�� $��7��$��5q�$��30��$��30��#���  $ Generar modelo������ # [ $ StartGeneratingCommand�$ OneWay=�.��  $
 Play=�$��20��$��20��� $��1�� ��  �� $
��auto ��� $
��auto ��� $��* �.+��� $��0��.��	Filter:d� $��1�� �� *IsFilterByEntityEnabled+�� $��2������ *DomainsCollection+ �� SelectedItem �� *SelectedFilterDomain+�� -    ��Name$  TargetEntitiesScrollViewer��$��2��$��5q�.���� -    ��Name$ TargetEntitiesList������ *TargetEntitiesCollection+ �� *SelectedEntity+���� .R��  �� $
��auto ��� $��* ��� $
��auto �.+�d� $��6q�$��0�� �� *
IsSelected+�� $��2��$��Center=����� *LogicalName+�� $��Right=�$��Stretch=�$��1�� �� ResizeBehavior$ PreviousAndNext=�$��5��
�������� $��2��y��� *IsCommonDetailShowing+ �� 	Converter# [ .+��� $��Editar entidad��.���  �� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��* ��� $
��auto � �� $
��auto ��� $��* ��� $
��auto �.+��� $��0��$��0��$��5q�$��Center=�$��Right=�Global enums output file:�� $��0��$��1��$��Center=����� *(&CommonTargetData.GlobalEnumsOutputFile+�� $��0��$��2��$��5q�$��Center=�ECRuta donde se guardará el archio de global enums cuando se ejecute�� $��1��$��0��$��5q�$��Center=�$��Right=�Trim prexies:d� $��1��$��1��$��Center=� �� *CommonTargetData.TrimPrefix+�� $��1��$��2��$��5q�$��Center=�20Eliminar prefijos de nombres de campo (scm, etc)�� $��2��$��0��$��5q�$��Center=�$��Right=�	Prefixes:�� $��2��$��1��$��Center=����� *TrimPrefixesInput+�� $��2��$��2��$��5q�$��Center=�75Prefijos que se eliminarán (separados por comas (,))�� $��3��$��0��$��5q�$��Center=�$��Right=�Capitalizar nombres:d� $��3��$��1��$��Center=� �� *CommonTargetData.Capitalize+�� $��3��$��2��$��5q�$��Center=�&$Intenta formar nombres en PascalCase� $��4��$��0��$��4�� �� $��* ��� $��* �.+��� $-��'Mapping campos para todas las entidades��$��0��$��3q�.���  �� $��* � �� $
��auto ��� $��* �.+�� $��1��$��0��$��2�� �� $
��auto ��� $��* �.+�� $��0�� �� $
��auto ��� $
��auto ��� $��* �.+��� $��5q�$��30��$��30��$��0��#���  $��Center=�$* $Añadir mapping común para Atributo������ # [ $ AddCommonMappingCommand�$ OneWay=�.��  $	 Add=�$��20��$��20���� $��1��$��5q�$��Center=�.��	Filter:�� $��2������ *FilterCommonMappingInput+ �� UpdateSourceTrigger$ PropertyChanged=�&   System.Windows.Controls.DataGrid  -     Name$ CommonMappingGrid��$��1��   AutoGenerateColumns .     CanUserAddRows .    CanUserDeleteRows . ���� *#!CommonMappingsEditablesCollection+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,	  &System.Windows.Controls.DataGridColumn 	  Header$ CRM Logical name��1
  +System.Windows.Controls.DataGridBoundColumn 
  Binding �� *Key+  $ Property name for C#�� �� *Value+  $ Source��  	  
IsReadOnly  .  �� *Source+�� $��Mapping picklist globales��$��1��$��3q�.���  �� $��* � �� $
��auto ��� $��* �.+�� $��1��$��0��$��2�� �� $
��auto ��� $��* �.+�� $��0�� �� $
��auto ��� $
��auto ��� $��* �.+��� $��5q�$��30��$��30��$��0��#���  $��Center=�$) #Añadir mapping comun para Picklist������ # [ $ AddCommonEnumCommand�$ OneWay=�.��  $	 Add=�$��20��$��20���� $��1��$��5q�$��Center=�.��	Filter:�� $��2������ *FilterCommonEnumInput+$ PropertyChanged=��� -   $  CommonEnumGridScrollViewer��$��1��.��  -   $ CommonEnumGrid�� .   .  . ���� * CommonEnumsEditablesCollection+   $ CRM Picklist name�� �� *Key+  $ Property name for C#�� �� *Value+�� $��5��$��Bottom=�$��2��$��Right=�$ 
Horizontal=�.+��� $��5q�$��30��$��30��#���  $ Cancelar cambios������ # [ $' !CancelEditCommonTargetDataCommand�$ OneWay=����� .��  $ Cancel=�$��20��$��20���� $��5q�$��30��$��30��#���  $ Guardar cambios������ # [ $( "ConfirmEditCommonDataTargetCommand�$ OneWay=����� .��  $ ContentSave=�$��20��$��20��� $��2��y��� *IsEntityDetailShowing+# [ .+��� $��Editar entidad��.���  �� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��* ��� $
��auto � �� $
��auto ��� $��* ��� $
��auto �.+��� $��0��$��0��$��5q�$��Center=�$��Right=�Entity domain name:�� $��0��$��1��$��Center=����� *!SelectedEntity.EntityDomainName+$ PropertyChanged=��� $��0��$��2��$��5q�$��Center=�!Nombre de dominio de la entidad�� $��1��$��0��$��5q�$��Center=�$��Right=�	Domain:�� $��1��$��1��$��Center=����� *DomainsCollection+ �� *SelectedDomain+�� $��1��$��2��$��5q�$��Center=� Dominio en el que se incluirá�� $��2��$��0��$��5q�$��Center=�$��Right=�Output file:�� $��2��$��1��$��Center=����� *SelectedEntity.OutputFile+$ PropertyChanged=�! �� 
IsReadOnly! �� *IsDefaultOutputFile+� $��2��$��2��$��5q�$��Center=� �� $��* ��� $
��auto �.+�d� $��0�� �� *IsDefaultOutputFile+�� $��1��Use default output path�� $��3��$��0��$��5q�$��Center=�$��Right=�
Mapping:�� $��3��$��1��$��2��$��5q�$��Center=�[YMapping que se aplicará para sustituir nombres de campos en CRM con nombres en CamelCase� $��4��$��0��$��3�� �� $��* ��� $��* �.+��� $��Mapping campos de entidad��$��0��$��3q�.���  �� $��* � �� $
��auto ��� $��* �.+�� $��1��$��0��$��2�� �� $
��auto ��� $��* �.+�� $��0�� �� $
��auto ��� $
��auto ��� $��* �.+��� $��5q�$��30��$��30��$��0��#���  $��Center=�$# Añadir mapping para Atributo������ # [ $ AddMappingCommand�$ OneWay=����� .��  $	 Add=�$��20��$��20���� $��1��$��5q�$��Center=�.��	Filter:�� $��2������ *FilterMappingInput+$ PropertyChanged=��� -   $ MappingGridScrollViewer��$��1��.��  -   $ MappingGrid�� .   .  . ���� *MappingsEditablesCollection+   $ CRM Logical name�� �� *Key+  $ Property name for C#�� �� *Value+  $ Source��  .  �� *Source+�� $!��Mapping picklist de entidad��$��1��$��3q�.���  �� $��* � �� $
��auto ��� $��* �.+�� $��1��$��0��$��2�� �� $
��auto ��� $��* �.+�� $��0�� �� $
��auto ��� $
��auto ��� $��* �.+��� $��5q�$��30��$��30��$��0��#���  $��Center=�$# Añadir mapping para Picklist������ # [ $ AddEnumCommand�$ OneWay=�.��  $	 Add=�$��20��$��20���� $��1��$��5q�$��Center=�.��	Filter:�� $��2������ *FilterEnumInput+$ PropertyChanged=��� -	   $ EnumGridScrollViewer��$��1��.��  -
   $ EnumGrid�� .   .  . ���� *EnumsEditablesCollection+   $ CRM Picklist name�� �� *Key+  $ Property name for C#�� �� *Value+�� $��5��$��Bottom=�$��2��$��Right=�$ 
Horizontal=�.+��� $��5q�$��30��$��30��#���  $ Cancelar cambios������ # [ $# CancelEditTargetEntityCommand�$ OneWay=�.��  $ Cancel=�$��20��$��20���� $��5q�$��30��$��30��#���  $ Guardar cambios������ # [ $$ ConfirmEditTargetEntityCommand�$ OneWay=�.��  $ ContentSave=�$��20��$��20��� $��2��y��� *IsAddingNewEntity+# [ .+��� $��Añadir nueva entidad��.���  �� $
��auto ��� $
��auto ��� $
��auto ��� $��* � �� $
��auto ��� $��* �.+��� $��0��$��5q�$��Center=�Logical name en CRM:�� $��1������ *NewEntityName+$ PropertyChanged=��� $��1��$��0��$��5q�$��Center=�$��Right=�	Domain:�� $��1��$��1��$��Center=����� *DomainsCollection+ �� *SelectedDomain+�� $��0��$��5q�$��2��$��Center=�Nombre de entidad:�� $��1��$��2������ *NewEntityDisplayName+$ PropertyChanged=��� $��3��$��Bottom=�$��1��$��Right=�$ 
Horizontal=�.+��� $��5q�$��30��$��30��#���  $ Cancelar cambios������ # [ $ CancelAddEntityCommand�$ OneWay=�.��  $ Cancel=�$��20��$��20���� $��5q�$��30��$��30��#���  $ Guardar cambios������ # [ $ AddEntityCommand�$ OneWay=�.��  $	 Add=�$��20��$��20��