﻿using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using HotelManagement.Utilities;
using HotelManagement.Utils;
using HotelManagement.View.CustomMessageBoxWindow;
using HotelManagement.View.Staff.RoomCatalogManagement.RoomPayment;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HotelManagement.ViewModel.StaffVM.RoomCatalogManagementVM
{
    public partial class RoomCatalogManagementVM : BaseVM
    {


        private List<BillDTO> _ListBill;
        public List<BillDTO> ListBill
        {
            get { return _ListBill; }
            set { _ListBill = value; OnPropertyChanged(); }
        }
        
     
        private ObservableCollection<string> _ListPaymentRoomNumber;
        public ObservableCollection<string> ListPaymentRoomNumber
        {
            get { return _ListPaymentRoomNumber; }
            set { _ListPaymentRoomNumber = value; OnPropertyChanged(); }
        }
        private ObservableCollection<ProductUsingDTO> _ListProductPayment;
        public ObservableCollection<ProductUsingDTO> ListProductPayment
        {
            get { return _ListProductPayment; }
            set { _ListProductPayment = value; OnPropertyChanged(); }
        }
        private List<RentalContractDTO> _ListRentalContractByCustomer;
        public List<RentalContractDTO> ListRentalContractByCustomer
        {
            get { return _ListRentalContractByCustomer; }
            set { _ListRentalContractByCustomer = value; OnPropertyChanged(); }
        }
       
        private BillDTO _BillPayment;
        public BillDTO BillPayment
        {
            get { return _BillPayment; }
            set { _BillPayment = value; OnPropertyChanged(); }
        }
      
        private string _StaffName;
        public string StaffName
        {
            get { return _StaffName; }
            set { _StaffName = value; OnPropertyChanged(); }
        }

        private ObservableCollection<TroubleByCustomerDTO> _ListTroubleByCustomer;
        public ObservableCollection<TroubleByCustomerDTO> ListTroubleByCustomer
        {
            get { return _ListTroubleByCustomer; }
            set { _ListTroubleByCustomer = value; OnPropertyChanged(); }
        }
        public string CreateDateStr
        {
            get
            {
                return DateTime.Today.ToString("dd/MM/yyyy");
            }
        }
        private double _TotalMoneyPayment;
        public double TotalMoneyPayment
        {
            get { return _TotalMoneyPayment; }
            set { _TotalMoneyPayment = value; OnPropertyChanged(); }
        }
        private string _TotalMoneyPaymentStr;
        public string TotalMoneyPaymentStr
        {
            get { return _TotalMoneyPaymentStr; }
            set { _TotalMoneyPaymentStr = value; OnPropertyChanged(); }
        }
        public string HotelPhone
        {
            get
            {
                return HOTEL_INFO.PHONE;
            }
        }
       
        public async Task Payment()
        {
            BillPayment = await BillService.Ins.GetBillByRentalContract(SelectedRoom.RentalContractId);
            RoomBill wd = new RoomBill();

            TotalMoneyPayment = 0;
            wd.ShowDialog();

           
        }
        
        public async Task SaveBillFunc(RoomBill p)
        {
            BillDTO roomBillDTO = new BillDTO
            {
                RentalContractId = BillPayment.RentalContractId,
                StaffId = CurrentStaff.StaffId,
                TotalPrice = BillPayment.TotalPriceTemp,
                CreateDate = DateTime.Now,
            };
            (bool isSucceed, string message) = await BillService.Ins.SaveBill(roomBillDTO);
            if (isSucceed)
            {
                CustomMessageBox.ShowOk(message, "Thông báo", "Ok", View.CustomMessageBoxWindow.CustomMessageBoxImage.Success);
                (bool isSucceed2, string message2) = await RoomService.Ins.ChangeRoomStatus(BillPayment.RoomId, BillPayment.RentalContractId);
                if (isSucceed2)
                {
                    await ReloadRoom();
                    p.Close();
                    RoomWindow.Close();
                }
                else
                {
                    CustomMessageBox.ShowOk(message2, "Lỗi", "OK", CustomMessageBoxImage.Error);
                }
                p.Close();
            }
            else
            {
                CustomMessageBox.ShowOk(message, "Thông báo", "Ok", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
        }
        public async Task LoadRoomBillFunc()
        {

            ListProductPayment = new ObservableCollection<ProductUsingDTO>(BillPayment.ListListProductPayment);
            ListProductPayment.Insert(0, new ProductUsingDTO
                {
                    ProductName = BillPayment.RoomName,
                    UnitPrice = BillPayment.RentalPrice,
                    Quantity = BillPayment.DayNumber,
                });


            TotalMoneyPayment = (double)BillPayment.TotalPriceTemp;
            TotalMoneyPaymentStr = Helper.FormatVNMoney2(TotalMoneyPayment);

        }
       
    }
}
