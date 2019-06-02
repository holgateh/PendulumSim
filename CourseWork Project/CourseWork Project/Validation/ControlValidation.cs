using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Text.RegularExpressions;


namespace CourseWork_Project.Validation
{
    static class ControlValidation 
    {
        //Regex pattern used when the text is being written.
        static string pattern = @"^\-?\d+(\.\d\d?\d?)?$";

        //Regex pattern used when the text has been entered.
        static string pattern2 = @"^\-?(\d+)?(\.\d?\d?\d?)?$";
        
        //Setup regex objects.
        static Regex regex = new Regex(pattern);
        static Regex regexPartialInput = new Regex(pattern2);

        //Checks that the text being entered is valid.
        public static void TextBoxValueChanged(TextBox textBox, Slider slider)
        {
            double textValue = 0;

            //If the text box content is a match...
            if (regex.IsMatch(textBox.Text))
            {
                textValue = Double.Parse(textBox.Text);

                //If the  parsed value is within the allowed range...
                if (textValue >= slider.Minimum && textValue <= slider.Maximum)
                {
                    slider.Value = Double.Parse(textValue.ToString("0.###"));

                }//If the text box content is not empty...  
                
            }//If the text box content is not match to the partial input regex... and is not empty.
            else if(!regexPartialInput.IsMatch(textBox.Text) && textBox.Text != "")          
            {
                string messageText = " is not a number!";
                MessageBox.Show(textBox.Text + messageText, "Error", MessageBoxButton.OK);
                textBox.Text = slider.Minimum.ToString("0.##");

            }
            
        }

        //Checks that the text that has been entered into the text box is valid.
        public static void TextBoxLostFocus(TextBox textBox, Slider slider)
        {
            double textValue = 0;

            //If the text box content is a match...
            if (regex.IsMatch(textBox.Text))
            {
                textValue = Double.Parse(textBox.Text);

                //If the  parsed value is within the allowed range...
                if (textValue >= slider.Minimum && textValue <= slider.Maximum)
                {
                    slider.Value = Double.Parse(textValue.ToString("0.###"));

                }//If the text box content is not empty... 
                else if (textBox.Text != "")
                {
                    string messageText = " is out of range!";

                    MessageBox.Show(textBox.Text + messageText, "Error", MessageBoxButton.OK);

                    if (textValue > slider.Maximum)
                    {
                        textValue = slider.Maximum;
                        textBox.Text = slider.Maximum.ToString("0.##");
                        slider.Value = textValue;
                    }
                    else if (textValue < slider.Minimum)
                    {
                        textValue = slider.Minimum;
                        textBox.Text = slider.Minimum.ToString("0.##");
                        slider.Value = textValue;
                    }


                }
            }
            else if(textBox.Text != "")
            {
                string messageText = " is not a number!";
                MessageBox.Show(textBox.Text + messageText, "Error", MessageBoxButton.OK);
                textBox.Text = slider.Minimum.ToString("0.##");

            }
            else
            {
                string messageText = "Please enter a number!";
                MessageBox.Show(textBox.Text + messageText, "Error", MessageBoxButton.OK);
                textBox.Text = slider.Minimum.ToString("0.##");
            }
        }
    }
}
