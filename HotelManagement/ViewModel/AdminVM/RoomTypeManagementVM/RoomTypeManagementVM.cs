﻿using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using HotelManagement.Model.Services;
using HotelManagement.View.CustomMessageBoxWindow;
using Microsoft.Office.Interop.Excel;
using HotelManagement.View.Admin.RoomTypeManagement;
using System.Windows;
using HotelManagement.Utils;
using System.Data.Entity.Core;
using HotelManagement.Model;
using System.Data.Entity;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.Entity.Validation;

namespace HotelManagement.ViewModel.AdminVM.RoomTypeManagementVM
{
    public partial class RoomTypeManagementVM : BaseVM
    {
        private string _roomTypeID;
        public string RoomTypeID
        {
            get { return _roomTypeID; }
            set { _roomTypeID = value; OnPropertyChanged(); }
        }

        private string _roomTypeName;
        public string RoomTypeName
        {
            get { return _roomTypeName; }
            set { _roomTypeName = value; OnPropertyChanged(); }
        }

        private string _roomTypePrice;
        public string RoomTypePrice
        {
            get { return _roomTypePrice; }
            set { _roomTypePrice = value; OnPropertyChanged(); }
        }

        private string _maxNumberGuest;
        public string MaxNumberGuest
        {
            get { return _maxNumberGuest; }
            set { _maxNumberGuest = value; OnPropertyChanged(); }
        }

        private string _numberGuestForUnitPrice;
        public string NumberGuestForUnitPrice
        {
            get { return _numberGuestForUnitPrice; }
            set { _numberGuestForUnitPrice = value; OnPropertyChanged(); }
        }

        private RoomTypeDTO _selectedItem;
        public RoomTypeDTO SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        private bool isloadding;
        public bool IsLoadding
        {
            get { return isloadding; }
            set { isloadding = value; OnPropertyChanged(); }
        }

        private bool isSaving;
        public bool IsSaving
        {
            get { return isSaving; }
            set { isSaving = value; OnPropertyChanged(); }
        }

        private ObservableCollection<RoomTypeDTO> _roomTypeList;
        public ObservableCollection<RoomTypeDTO> RoomTypeList
        {
            get => _roomTypeList;
            set
            {
                _roomTypeList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SurchargeFeeDTO> _listSurchargeRate;
        public ObservableCollection<SurchargeFeeDTO> ListSurchargeRate
        {
            get => _listSurchargeRate;
            set
            {
                _listSurchargeRate = value;
                OnPropertyChanged();
            }
        }

        private System.Windows.Window windowAdd;
        private System.Windows.Window windowEdit;
        public ICommand FirstLoadCM { get; set; }
        public ICommand CloseCM { get; set; }
        public ICommand CloseCM2 { get; set; }
        public ICommand LoadAddRoomTypeCM { get; set; }
        public ICommand LoadDeleteRoomTypeCM { get; set; }
        public ICommand LoadEditRoomTypeCM { get; set; }
        public ICommand SaveRoomTypeCM { get; set; }
        public ICommand UpdateRoomTypeCM { get; set; }
        public ICommand LoadSurchargeFeeCM { get; set; }
        public ICommand LoadEditSurchargeFeeCM { get; set; }


        public RoomTypeManagementVM()
        {
            FirstLoadCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {

                RoomTypeList = new ObservableCollection<RoomTypeDTO>();
                try
                {
                    IsLoadding = true;
                    RoomTypeList = new ObservableCollection<RoomTypeDTO>(await Task.Run(() => RoomTypeService.Ins.GetAllRoomType()));
                    IsLoadding = false;
                }
                catch (System.Data.Entity.Core.EntityException e)
                {
                    Console.WriteLine(e);
                    CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                }
            });
            LoadAddRoomTypeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                RenewWindowData();
                AddRoomType addRoomType = new AddRoomType();
                windowAdd = addRoomType;
                addRoomType.ShowDialog();
            });
            LoadSurchargeFeeCM = new RelayCommand<System.Windows.Window>((p) => { if (IsSaving) return false; return true; }, async (p) =>
            {
                if (!IsValidData(p))
                {
                    CustomMessageBox.ShowOk("Vui lòng nhập đủ thông tin!", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                    return;
                }
                else
                {
                    
                    if (CheckExistedRoomTypeName(RoomTypeName,"add",""))
                    {
                        
                        CustomMessageBox.ShowOk("Tên loại phòng đã tồn tại. Vui lòng nhập tên loại phòng khác !!!", "Thông báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        double price_roomtype;
                        int max_quantity;
                        int number_guest;

                        bool isDoublePrice = double.TryParse(RoomTypePrice, out price_roomtype);
                        bool isIntMaxQuantity = Int32.TryParse(MaxNumberGuest, out max_quantity);
                        bool isNumberGuest = Int32.TryParse(NumberGuestForUnitPrice, out number_guest);

                        if (!isDoublePrice || price_roomtype <= 0)
                        {
                            CustomMessageBox.ShowOk("Giá phải là số dương", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                            return;
                        }
                        if (!isIntMaxQuantity || max_quantity <= 0)
                        {
                            CustomMessageBox.ShowOk("Số khách tối đa là một số nguyên dương", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                            return;
                        }
                        if (!isNumberGuest || number_guest <= 0)
                        {
                            CustomMessageBox.ShowOk("Số khách tính đơn giá là một số nguyên dương", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                            return;
                        }
                        
                        if (Int32.Parse(MaxNumberGuest) < Int32.Parse(NumberGuestForUnitPrice))
                        {
                            CustomMessageBox.ShowOk("Số khách tính đơn giá phải nhỏ hơn hoặc bằng số khách tối đa !!!", "Thông báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                            return;
                        }
                        if (Int32.Parse(MaxNumberGuest) == Int32.Parse(NumberGuestForUnitPrice))
                        {
                            IsSaving = true;
                            await SaveRoomTypeFunc(p, windowAdd);
                            IsSaving = false;
                            return;
                        }
                        try
                        {
                            SurchargeFee surchargeFee = new SurchargeFee();
                            ListSurchargeRate = new ObservableCollection<SurchargeFeeDTO>();
                            GetValueListSurchargeFee();
                            surchargeFee.ShowDialog();
                        }
                        catch (EntityException ex)
                        {
                            CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                        }
                        catch (Exception e)
                        {
                            CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                        }
                    }
                }
            });
            SaveRoomTypeCM = new RelayCommand<System.Windows.Window>((p) => { if (IsSaving) return false; return true; }, async (p) =>
            {
                IsSaving = true;
                await SaveRoomTypeFunc(p, windowAdd);              
                IsSaving = false;
            });
            LoadEditRoomTypeCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                EditRoomType w1 = new EditRoomType();
                LoadEditRoomType();
                windowEdit = w1;
                w1.ShowDialog();
            });
            LoadEditSurchargeFeeCM = new RelayCommand<System.Windows.Window>((p) => { if (IsSaving) return false; return true; }, async (p) =>
            {
                if (!IsValidData2())
                {
                    CustomMessageBox.ShowOk("Vui lòng nhập đủ thông tin!", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                    return;
                }
                else
                {
                    if (CheckExistedRoomTypeName(RoomTypeName,"edit",RoomTypeID))
                    {
                        CustomMessageBox.ShowOk("Tên loại phòng đã tồn tại. Vui lòng nhập tên loại phòng khác !!!", "Thông báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        double price_roomtype;
                        int max_quantity;
                        int number_guest;

                        bool isDoublePrice = double.TryParse(RoomTypePrice, out price_roomtype);
                        bool isIntMaxQuantity = Int32.TryParse(MaxNumberGuest, out max_quantity);
                        bool isNumberGuest = Int32.TryParse(NumberGuestForUnitPrice, out number_guest);

                        if (!isDoublePrice || price_roomtype <= 0)
                        {
                            CustomMessageBox.ShowOk("Giá phải là số dương", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                            return;
                        }
                        if (!isIntMaxQuantity || max_quantity <= 0)
                        {
                            CustomMessageBox.ShowOk("Số khách tối đa là một số nguyên dương", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                            return;
                        }
                        if (!isNumberGuest || number_guest <= 0)
                        {
                            CustomMessageBox.ShowOk("Số khách tính đơn giá là một số nguyên dương", "Cảnh báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                            return;
                        }

                        if (Int32.Parse(MaxNumberGuest) < Int32.Parse(NumberGuestForUnitPrice))
                        {
                            CustomMessageBox.ShowOk("Số khách tính đơn giá phải nhỏ hơn hoặc bằng số khách tối đa !!!", "Thông báo", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Warning);
                            return;
                        }
                        if (Int32.Parse(MaxNumberGuest) == Int32.Parse(NumberGuestForUnitPrice))
                        {
                            ListSurchargeRate = null;
                            IsSaving = true;
                            await UpdateRoomTypeFunc(p, windowEdit);
                            IsSaving = false;
                            return;
                        }
                        try
                        {
                            EditSurchargeFee surchargeFee = new EditSurchargeFee();

                            ListSurchargeRate = new ObservableCollection<SurchargeFeeDTO>();

                            GetValueListEditSurchargeFee(SelectedItem.RoomTypeId);

                            surchargeFee.ShowDialog();
                        }
                        catch (EntityException ex)
                        {
                            CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                        }
                        catch (Exception e)
                        {
                            CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                        }
                    }
                }
            });
            UpdateRoomTypeCM = new RelayCommand<System.Windows.Window>((p) => { if (IsSaving) return false; return true; }, async (p) =>
            {
                IsSaving = true;
                await UpdateRoomTypeFunc(p, windowEdit);
                IsSaving = false;
            });
            LoadDeleteRoomTypeCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {

                string message = "Bạn có chắc muốn xoá phòng này không? Dữ liệu không thể phục hồi sau khi xoá!";
                CustomMessageBoxResult kq = CustomMessageBox.ShowOkCancel(message, "Cảnh báo", "Xác nhận", "Hủy", CustomMessageBoxImage.Warning);

                if (kq == CustomMessageBoxResult.OK)
                {
                    IsLoadding = true;

                    (bool successDeleteRoomType, string messageFromDelRoomType) = await RoomTypeService.Ins.DeleteRoomType(SelectedItem.RoomTypeId);

                    IsLoadding = false;

                    if (successDeleteRoomType)
                    {
                        LoadRoomTypeListView(Operation.DELETE);
                        SelectedItem = null;
                        CustomMessageBox.ShowOk(messageFromDelRoomType, "Thông báo", "OK", CustomMessageBoxImage.Success);
                    }
                    else
                    {
                        CustomMessageBox.ShowOk(messageFromDelRoomType, "Lỗi", "OK", CustomMessageBoxImage.Error);
                    }
                }
            });
            CloseCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                SelectedItem = null;
                p.Close();
            });
            CloseCM2 = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            });
        }

        public void GetValueListSurchargeFee()
        {
            using (HotelManagementEntities db = new HotelManagementEntities())
            {
                for (int i = 0; i < Int32.Parse(MaxNumberGuest) - Int32.Parse(NumberGuestForUnitPrice); i++)
                {
                    SurchargeFeeDTO srDTO = new SurchargeFeeDTO();
                    srDTO.CustomerOutIndex = i + 1;
                    srDTO.Rate = "";
                    ListSurchargeRate.Add(srDTO);
                }
            }
        }
        public async void GetValueListEditSurchargeFee(string id)
        {
            using (HotelManagementEntities db = new HotelManagementEntities())
            {
                for (int i = 0; i < Int32.Parse(MaxNumberGuest) - Int32.Parse(NumberGuestForUnitPrice); i++)
                {
                    SurchargeFeeDTO srDTO = new SurchargeFeeDTO();
                    srDTO.CustomerOutIndex = i + 1;
                    SurchargeRate sr = await db.SurchargeRates.FirstOrDefaultAsync(item => item.CustomerOutIndex == srDTO.CustomerOutIndex && item.RoomTypeId == id);
                    if (sr == null)
                        srDTO.Rate = "";
                    else
                        srDTO.Rate = sr.Rate.ToString();
                    ListSurchargeRate.Add(srDTO);
                }
            }
        }
        public async void ReloadListView()
        {
            RoomTypeList = new ObservableCollection<RoomTypeDTO>();
            try
            {
                IsLoadding = true;
                RoomTypeList = new ObservableCollection<RoomTypeDTO>(await Task.Run(() => RoomTypeService.Ins.GetAllRoomType()));
                IsLoadding = false;
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                Console.WriteLine(e);
                CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
        }
        public void LoadRoomTypeListView(Operation oper = Operation.READ, RoomTypeDTO rt = null)
        {
            switch (oper)
            {
                case Operation.CREATE:
                    RoomTypeList.Add(rt);
                    break;
                case Operation.UPDATE:
                    var roomTypeFound = RoomTypeList.FirstOrDefault(x => x.RoomTypeId == rt.RoomTypeId);
                    RoomTypeList[RoomTypeList.IndexOf(roomTypeFound)] = rt;
                    break;
                case Operation.DELETE:
                    for (int i = 0; i < RoomTypeList.Count; i++)
                    {
                        if (RoomTypeList[i].RoomTypeId == SelectedItem?.RoomTypeId)
                        {
                            RoomTypeList.Remove(RoomTypeList[i]);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public void RenewWindowData()
        {
            RoomTypeName = null;
            RoomTypePrice = null;
            MaxNumberGuest = null;
            NumberGuestForUnitPrice = null;
        }
        public bool CheckExistedRoomTypeName(string rt_name, string type, string id)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {   
                    if (type=="add")
                    {
                        //RoomType rt = context.RoomTypes.Where((RoomType RoomType) => RoomType.RoomTypeName == rt_name).FirstOrDefault();
                        var rt = context.RoomTypes.FirstOrDefault(x => x.RoomTypeName == rt_name);
                        if (rt != null)
                        {
                            return true;
                        }
                        else return false;
                    }
                    else
                    {
                        RoomType rt = context.RoomTypes.Where((RoomType RoomType) => RoomType.RoomTypeId != id && RoomType.RoomTypeName == rt_name).FirstOrDefault();
                        if (rt != null)
                        {
                            return true;
                        }
                        else return false;
                    }
                    
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                CustomMessageBox.ShowOk("DbEntityValidationException", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                return false;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CustomMessageBox.ShowOk($"Error Server {e}", "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
                return false;
            }
            return false;
        }
        public bool IsValidData(System.Windows.Window p)
        {
            AddRoomType addRoomType = (AddRoomType)p;
            return !string.IsNullOrEmpty(RoomTypeName) && addRoomType.Gia.Text.Length > 0 && addRoomType.SoKhachTinhDonGia.Text.Length > 0 && addRoomType.SoKhachToiDa.Text.Length > 0;
        }
        public bool IsValidData2()
        {
            return !string.IsNullOrEmpty(RoomTypeName) && !string.IsNullOrEmpty(SelectedItem.RoomTypePrice.ToString()) && !string.IsNullOrEmpty(SelectedItem.MaxNumberGuest.ToString()) && !string.IsNullOrEmpty(SelectedItem.NumberGuestForUnitPrice.ToString());
        }

    }
}
