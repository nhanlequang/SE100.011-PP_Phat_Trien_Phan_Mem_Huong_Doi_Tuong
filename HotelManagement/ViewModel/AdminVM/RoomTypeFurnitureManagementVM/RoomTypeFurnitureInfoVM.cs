﻿using HotelManagement.DTOs;
using HotelManagement.Model.Services;
using HotelManagement.View.Admin;
using HotelManagement.View.CustomMessageBoxWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HotelManagement.ViewModel.AdminVM.RoomTypeFurnitureManagementVM
{
    public partial class RoomTypeFurnitureManagementVM
    {
        private ObservableCollection<FurnitureDTO> allFurnitureInRoom;
        public ObservableCollection<FurnitureDTO> AllFurnitureInRoom
        {
            get { return allFurnitureInRoom; }
            set { allFurnitureInRoom = value; OnPropertyChanged(); }
        }

        private ObservableCollection<FurnitureDTO> listFurnitureNeedDelete;
        public ObservableCollection<FurnitureDTO> ListFurnitureNeedDelete
        {
            get { return listFurnitureNeedDelete; }
            set { listFurnitureNeedDelete = value; OnPropertyChanged(); }
        }

        private FurnitureDTO furnitureCache;
        public FurnitureDTO FurnitureCache
        {
            get { return furnitureCache; }
            set { furnitureCache = value; OnPropertyChanged(); }
        }

        public ICommand FirstLoadInfoWindowCM { get; set; }
        public ICommand ChooseItemToListNeedDelete { get; set; }
        public ICommand RemoveItemToListNeedDelete { get; set; }
        public ICommand ChooseAllFurnitureToDeleteCM { get; set; }
        public ICommand DeleteListFurnitureCM { get; set; }
        public ICommand CloseDeleteControlCM { get; set; }
        public ICommand SelectionFilterInfoChangeCM { get; set; }


        public async Task LoadFurniture()
        {
            (bool isSuccess, string messageReturn, List<FurnitureDTO> listFurnituresRoomReturn) = await Task.Run(() => FurnitureRoomTypeService.Ins.GetAllFurnituresIn(FurnituresRoomTypeCache));

            if (isSuccess)
            {
                FurnituresRoomTypeCache.ListFurnitureRoomType = new ObservableCollection<FurnitureDTO>(listFurnituresRoomReturn);
                foreach (var item in FurnituresRoomTypeCache.ListFurnitureRoomType)
                {
                    item.IsSelectedDelete = false;
                }
                AllFurniture = new ObservableCollection<FurnitureDTO>(listFurnituresRoomReturn);
                FurnitureList = new ObservableCollection<FurnitureDTO>(AllFurniture);
                FurnituresRoomTypeCache.SetQuantityAndStringTypeFurniture();
            }
            else
            {
                CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", View.CustomMessageBoxWindow.CustomMessageBoxImage.Error);
            }
        }
        public async Task DeleteListFurniture(Window p, AdminWindow adWD)
        {
            if (ListFurnitureNeedDelete.Count() == 0)
            {
                CustomMessageBox.ShowOk("Vui lòng chọn tiện nghi để xóa!", "Cảnh báo", "OK", CustomMessageBoxImage.Warning);
                return;
            }
            foreach (var item in ListFurnitureNeedDelete)
            {
                if (!item.IsDeleteLessThanInUse())
                {
                    CustomMessageBox.ShowOk("Số lượng cần xóa phải nhỏ hơn lượng tiện nghi trong phòng!", "Cảnh báo", "OK", CustomMessageBoxImage.Warning);
                    return;
                }
                if (item.DeleteInRoomQuantity < 0)
                {
                    CustomMessageBox.ShowOk("Số lượng cần xóa không được âm", "Cảnh báo", "OK", CustomMessageBoxImage.Warning);
                    return;
                }
            }
            if (CustomMessageBox.ShowOkCancel("Bạn có muốn xóa những tiện nghi được chọn ra khỏi phòng không?", "Cảnh báo", "Có", "Không", CustomMessageBoxImage.Warning)
                == CustomMessageBoxResult.OK)
            {
                (bool isSuccess, string messageReturn) = await Task.Run(() => FurnitureRoomTypeService.Ins.DeleteListFurnitureRoom(FurnituresRoomTypeCache.RoomTypeId, ListFurnitureNeedDelete));
                if (isSuccess)
                {
                    CustomMessageBox.ShowOk(messageReturn, "Thành công", "OK", CustomMessageBoxImage.Success);
                    FurnituresRoomTypeCache.DeleteListFurniture(ListFurnitureNeedDelete);
                    FurnituresRoomTypeCache.SetQuantityAndStringTypeFurniture();
                    ListFurnitureNeedDelete.Clear();
                }
                else
                {
                    CustomMessageBox.ShowOk(messageReturn, "Lỗi", "OK", CustomMessageBoxImage.Error);
                }
            }
            p.Close();
            adWD.MaskOverSideBar.Visibility = Visibility.Collapsed;
        }
    }
}