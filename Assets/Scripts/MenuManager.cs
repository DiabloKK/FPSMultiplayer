using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    /* 
    Khai báo biến tĩnh, chỉ có duy nhất một phiên bản của nó tồn tại trong chương trình
    và có thể truy cập từ bất kỳ đâu trong cùng phạm vi lớp. 
    */
    public static MenuManager Instance;

    // Khai báo danh sách các menu (khởi tạo trong unity)
    [SerializeField] Menu[] menus;
  
    // Awake() là phương thức được gọi tự động khi một đối tượng được khởi tạo và sẵn sàng sử dụng trong scene
    void Awake()
    {
        Instance = this;
    }

    // Mở menu theo đúng tham số menuName và đóng các tham số còn lại nếu menu đó đang mở 
    public void OpenMenu(string menuName) {
        for(int i=0; i<menus.Length; i++) {
            if(menus[i].menuName == menuName) {
                OpenMenu(menus[i]);
            } else if(menus[i].open) {
                CloseMenu(menus[i]);
            }
        } 
    }

    public void OpenMenu(Menu menu) {
        for(int i=0; i<menus.Length; i++) {
            if(menus[i].open) {
                CloseMenu(menus[i]);
            }
        } 
        menu.Open();
    }

    public void CloseMenu(Menu menu) {
        menu.Close();
    }

}
