   M S B A M L   `   `   `  ����B  >WPF.MDI, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null2&clr-namespace:WPF.MDI;assembly=WPF.MDIWPF.MDI  5   	   M ICourseWork Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:CourseWork_ProjectCourseWork_Project 5   	   #   CourseWork_Project.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   6mdi&clr-namespace:WPF.MDI;assembly=WPF.MDI     5   	   2local clr-namespace:CourseWork_Project     5   	     � Title$  Harmonic Motion Simulator��5   	   $	��715��6+   $
��1315��68    � Icon$ Icons/MainIcon.PNG��5   	    � 
ResizeMode$ NoResize=�6#   .��5
      �  � ColumnDefinitions 5   
   �� 5       � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $��50 �6   �� 5      $
��574* �6   .+�5   
   '  !CourseWork_Project.ToolBarControl  -   5          Name$ ToolBar��5      '  !CourseWork_Project.ToolBarControl   ZoomResetButtonClicked$$ ToolBar_ZoomResetButtonClicked��5   &   !   PenToolToggleButtonClicked$( "ToolBar_PenToolToggleButtonClicked��5   &   &   ClearPenToolToggleButtonClicked$- 'ToolBar_ClearPenToolToggleButtonClicked��5   &   "	   LineToolToggleButtonClicked$)	 #ToolBar_LineToolToggleButtonClicked��5   &   $
��Auto��5   &   $
��Auto��64   $��Stretch=�6A   $	��Top=�5   &   $��1��5   &   ]� 5   
   $��2��6   $��2��6!   .+�5         WPF.MDI.MdiContainer  -   5        
  Name$
 	Container��5      5      o� 5   
    o� 
IsMainMenu . $��0��6!   .@�5      m� -   5        $��_Reset Simulation��5       � ToolTip$7 1Reloads/Resets the Currently Selected Simulation.��5!      m� 5"      $��_Select Simulation��6   $$ Select a Different Simulation.��63   .@�5#      m� -   5        $��PendulumSimMenuItem��5#      $��_Pendulum Simulation��68   5$      �� 5%      m� 5&      -   5        $��SpringSimMenuItem��5&      $��_Spring Simulation��65   5'      �� 5(      m� 5)      -   5        $��DoublePendlumSimMenuItem��5)      $!��_Double Pendulum Simulation��6=   5*      5+      5,      5-      5/      