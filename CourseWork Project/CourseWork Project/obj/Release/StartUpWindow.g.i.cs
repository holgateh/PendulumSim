﻿#pragma checksum "..\..\StartUpWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5CBFB460F34C57C17CC192066944C74178FA8E1B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CourseWork_Project;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace CourseWork_Project {
    
    
    /// <summary>
    /// StartUpWindow
    /// </summary>
    public partial class StartUpWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 26 "..\..\StartUpWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PendulumOption;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\StartUpWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SpringPendulumOption;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\StartUpWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DoublePendulumOption;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\StartUpWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ExitOption;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CourseWork Project;component/startupwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\StartUpWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.PendulumOption = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\StartUpWindow.xaml"
            this.PendulumOption.Click += new System.Windows.RoutedEventHandler(this.PendulumOption_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.SpringPendulumOption = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\StartUpWindow.xaml"
            this.SpringPendulumOption.Click += new System.Windows.RoutedEventHandler(this.SpringPendulumOption_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.DoublePendulumOption = ((System.Windows.Controls.Button)(target));
            
            #line 37 "..\..\StartUpWindow.xaml"
            this.DoublePendulumOption.Click += new System.Windows.RoutedEventHandler(this.DoublePendulumOption_OnClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ExitOption = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\StartUpWindow.xaml"
            this.ExitOption.Click += new System.Windows.RoutedEventHandler(this.ExitOption_OnClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

