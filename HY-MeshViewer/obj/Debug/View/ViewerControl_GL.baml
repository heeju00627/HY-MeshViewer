   M S B A M L   `   `   `  ����R  NSharpGL.WPF, Version=2.4.0.0, Culture=neutral, PublicKeyToken=27fc851303210b27>.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPFSharpGL.WPF  5      H DHY-MeshViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:HY_MeshViewer.ViewHY_MeshViewer.View 5      A%clr-namespace:HY_MeshViewer.ViewModelHY_MeshViewer.ViewModel 5      )   #HY_MeshViewer.View.ViewerControl_GL   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      BsharpGL.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPF     5      2local clr-namespace:HY_MeshViewer.View     5      ;	viewmodel%clr-namespace:HY_MeshViewer.ViewModel     5        � 	Resources  5      1  +HY_MeshViewer.ViewModel.MainWindowViewModel   MainWindowViewModel��%   &	          5   
   6(   5       � DataContext 5        5   
   -   5        aa��5   (   .��5      �  � RowDefinitions 5   
   �� 5      -   5         �Name$ zeroRow��5      $��18 �6+   �� 5      -   5        $ firstRow��5      $��* �6,   �� 5      -   5        $ 	secondRow��5      $��18 �6-    � ColumnDefinitions 5   
   �� 5      $��18 �6   �� 5      $��* �6   �� 5      $��18 �6   .+�5   
   ]� $��1��6   $��1��6!   .+�5      � -   5        $��content��5      
������� � LayoutTransform 5      V� 5       .~�5!      �� -   5        scaleTransform��5!   )   �� 5"      -   5        rotateTransform��5"   *   5#      .+�5%         SharpGL.WPF.OpenGLControl  -   5        $��openGlControl��5%   (      DrawFPS .    RenderContextType$	 FBO=�5'   %   5(      5)      �� 5+   
   $��0��6   $��0��6!   $��13,0,0,0q�61   ��6C   �� *
N_triangle+�� 5,   
   $��0��6   $��2��6!   $��6,3,0,0q�61   y6E   �� 5-   
   $��2��6   $��2��6!   $��0,0,13,0q�61   z6F   �� 5.   
   $��2��6   $��0��6!   $��0,-6,0,0q�61   +-6G   �� 50   
   -	   5        $��
zoomSlider��50   .   $��1��6   $��0��6    ^� ToolTip$ 
zoomSlider��6@   	 �� Orientation$	 Vertical=�6d   
 � Minimum$	
 0.2Q�6{    � Value$ 1Q�6�    � Maximum$ 10Q�6�   �� 51   
   -
   5        $��xSlider��51   .   $��0��6   $��1��6   $ xSlider��6=   $	 
Horizontal=�6^   $

 -180Q�6w   $	 180Q�6�   �� 52   
   -   5        $��ySlider��52   .   $��1��6   $��2��6   $ ySlider��6=   $	 Vertical=�6^   $

 -180Q�6u   $	 180Q�6�   �� 53   
   -   5        $��zSlider��53   .   $��2��6   $��1��6   $ zSlider��6=   $	 
Horizontal=�6^   $

 -180Q�6w   $	 180Q�6�   54      55      