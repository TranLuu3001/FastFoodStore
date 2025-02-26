function searchEmployee() {
    // Lấy giá trị từ ô tìm kiếm
    const searchInput = document.getElementById('searchInput').value.toLowerCase();
    const employeeList = document.getElementById('employeeList');
    const rows = employeeList.getElementsByTagName('tr');

    // Lặp qua từng hàng trong danh sách nhân viên
    for (let i = 0; i < rows.length; i++) {
        const cells = rows[i].getElementsByTagName('td');
        // Kiểm tra nếu hàng có dữ liệu
        if (cells.length > 0) {
            const employeeCode = cells[1].textContent.toLowerCase(); // Mã nhân viên
            const employeeName = cells[2].textContent.toLowerCase(); // Họ và tên

            // Kiểm tra xem có khớp với ô tìm kiếm không
            if (employeeCode.includes(searchInput) || employeeName.includes(searchInput)) {
                rows[i].style.display = ""; // Hiện hàng nếu có khớp
            } else {
                rows[i].style.display = "none"; // Ẩn hàng nếu không khớp
            }
        }
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const rows = document.querySelectorAll('#employeeList tr');

    rows.forEach(row => {
        row.addEventListener('click', function () {
            const employeeCode = row.getAttribute('data-id');
            window.location.href = `Detail_Eployee.html?employee=${employeeCode}`;
        });
    });
    // Ngăn chặn sự kiện click của checkbox và td chứa checkbox ảnh hưởng đến hàng
    const checkboxes = document.querySelectorAll('.employee-checkbox');
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('click', function (event) {
            event.stopPropagation();  // Ngăn sự kiện click lan ra phần tử cha (hàng)
        });
    });

    // Xử lý click vào vùng chứa checkbox để chọn/deselect checkbox
    const checkboxCells = document.querySelectorAll('#employeeList td:first-child');
    checkboxCells.forEach(cell => {
        cell.addEventListener('click', function (event) {
            const checkbox = cell.querySelector('.employee-checkbox');
            // Đổi trạng thái chọn của checkbox khi nhấn vào ô chứa checkbox
            checkbox.checked = !checkbox.checked;
            event.stopPropagation();  // Ngăn sự kiện click lan ra phần tử cha (hàng)
        });
    });
    // Xử lý sự kiện chọn tất cả các checkbox
    const selectAllCheckbox = document.querySelector('.Allemployee-checkbox');
    const selectAllHeaderCell = document.querySelector('th input.Allemployee-checkbox');

    // Khi click vào vùng checkbox trong th, thay đổi trạng thái checkbox "Chọn tất cả"
    selectAllHeaderCell.closest('th').addEventListener('click', function (event) {
        if (event.target.tagName !== 'INPUT') { // Nếu không phải là phần tử input được click
            selectAllCheckbox.checked = !selectAllCheckbox.checked;
        }
        // Đặt tất cả checkbox con theo trạng thái của checkbox "Chọn tất cả"
        const allChecked = selectAllCheckbox.checked;
        checkboxes.forEach(checkbox => {
            checkbox.checked = allChecked;
        });
    });

    // Đảm bảo nếu nhấp trực tiếp vào checkbox trong th, trạng thái sẽ được giữ lại mà không thay đổi
    selectAllCheckbox.addEventListener('click', function (event) {
        const allChecked = selectAllCheckbox.checked;
        checkboxes.forEach(checkbox => {
            checkbox.checked = allChecked;  // Đặt tất cả checkbox theo trạng thái của "Chọn tất cả"
        });
    });
});