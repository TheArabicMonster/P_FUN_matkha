using Microsoft.VisualStudio.TestTools.UnitTesting;
using OxyPlot;
using OxyPlot.Wpf;
using RiftMetrics;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Controls;

namespace RiftMetrics.Tests
{
    [TestClass]
    public class MainWindowTests
    {
        
        [TestMethod]
        public void TestRegionSelectionChanged()
        {
            var thread = new Thread(() =>
            {
                // Arrange
                var mainWindow = new MainWindow();
                var comboBox = new ComboBox { ItemsSource = mainWindow.Regions };
                comboBox.SelectedItem = "Europe West";

                // Act
                mainWindow.ComboBox_SelectionChanged(comboBox, null);

                // Assert
                Assert.AreEqual("Europe West", mainWindow.invocateur.Region);
            });
        }

        [TestMethod]
        public void TestNomInvocateur_TextChanged()
        {
            var thread = new Thread(() =>
            {
                // Arrange
                var mainWindow = new MainWindow();
                var textBox = new TextBox { Text = "Summoner#1234" };

                // Act
                mainWindow.NomInvocateur_TextChanged(textBox, null);

                // Assert
                Assert.AreEqual("Summoner", mainWindow.invocateur.Nom);
                Assert.AreEqual("#1234", mainWindow.invocateur.Id);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        [TestMethod]
        public void TestReadJson()
        {
            var thread = new Thread(() =>
            {
                // Arrange
                var mainWindow = new MainWindow();
                string jsonContent = @"
                {
                    'Games': [
                        {
                            'player_trends': {
                                'Hourly': [
                                    { 'Day': '2023-10-01', 'Hour': '10:00', 'Players': 500 },
                                    { 'Day': '2023-10-01', 'Hour': '11:00', 'Players': 600 }
                                ]
                            }
                        }
                    ]
                }";

                string tempFilePath = Path.GetTempFileName();
                File.WriteAllText(tempFilePath, jsonContent);

                // Act
                var dataPoints = mainWindow.ReadJson(tempFilePath);

                // Assert
                Assert.AreEqual(2, dataPoints.Count);
                Assert.AreEqual(500, dataPoints[0].Y);
                Assert.AreEqual(600, dataPoints[1].Y);

                // Clean up
                File.Delete(tempFilePath);
            });
        }
    }
}
