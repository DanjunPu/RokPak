using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using System.Linq;
using CoreBluetooth;


namespace RokPak
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BlueTooth : ContentPage
    {

        private const int ScanTime = 5000;
        private const string DeviceName = "Charge 2";

        public BluetoothService bluetoothService;
        private bool alreadyScanned;
        private bool alreadyDiscovered;
        public BlueTooth()
        {
            InitializeComponent();
        }

        public async void OnConnectDeviceButtonClicked(object sender, EventArgs e)
        {
            Navigation.InsertPageBefore(new RokPak.BluetoothCancel(), this);
            await Navigation.PopAsync();
        }

        void OnClickCommand(object sender, EventArgs e)
        {
            Device.OpenUri(new System.Uri("https://RokPak.com"));
        }

        private async void StateChanged(object sender, CBCentralManagerState state)
        {
            if (!this.alreadyScanned && state == CBCentralManagerState.PoweredOn)
            {
                try
                {
                    this.alreadyScanned = true;
                    var connectedDevice = this.bluetoothService.GetConnectedDevices("180A")
                        ?.FirstOrDefault(x => x.Name.StartsWith(DeviceName, StringComparison.InvariantCulture));

                    if (connectedDevice != null)
                    {
                        this.DiscoveredDevice(this, connectedDevice);
                    }
                    else
                    {
                        await this.bluetoothService.Scan(ScanTime);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private async void DiscoveredDevice(object sender, CBPeripheral peripheral)
        {
            if (!this.alreadyDiscovered && peripheral.Name.StartsWith(DeviceName, StringComparison.InvariantCulture))
            {
                try
                {
                    this.alreadyDiscovered = true;

                    await this.bluetoothService.ConnectTo(peripheral);

                    var service = await this.bluetoothService.GetService(peripheral, "180A");
                    if (service != null)
                    {
                        var characteristics = await this.bluetoothService.GetCharacteristics(peripheral, service, ScanTime);
                        foreach (var characteristic in characteristics)
                        {
                            var value = await this.bluetoothService.ReadValue(peripheral, characteristic);
                            Debug.WriteLine($"{characteristic.UUID.Description} = {value}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    this.bluetoothService.Disconnect(peripheral);
                }
            }
        }
    }
}
