﻿#pragma checksum "..\..\Preferences.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "DC5A251362B02CB6F34BBD51B926383D26FC9CBA"
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
    /// PreferencesWindow
    /// </summary>
    public partial class PreferencesWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "..\..\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ChangeButtonOne;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ChangeButtonTwo;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ChangeButtonThree;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ColourCanvasOne;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ColourCanvasTwo;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ColourCanvasThree;
        
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
            System.Uri resourceLocater = new System.Uri("/CourseWork Project;component/preferences.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Preferences.xaml"
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
            this.ChangeButtonOne = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\Preferences.xaml"
            this.ChangeButtonOne.Click += new System.Windows.RoutedEventHandler(this.ChangeButtonOne_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ChangeButtonTwo = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\Preferences.xaml"
            this.ChangeButtonTwo.Click += new System.Windows.RoutedEventHandler(this.ChangeButtonTwo_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ChangeButtonThree = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\Preferences.xaml"
            this.ChangeButtonThree.Click += new System.Windows.RoutedEventHandler(this.ChangeButtonThree_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ColourCanvasOne = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.ColourCanvasTwo = ((System.Windows.Controls.Canvas)(target));
            return;
            case 6:
            this.ColourCanvasThree = ((System.Windows.Controls.Canvas)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

