// ===============================================
// VLXD MANAGER - Main JavaScript
// ===============================================

// Clock
function updateClock() {
    const now = new Date();
    const el = document.getElementById('clock');
    if (el) {
        el.textContent = now.toLocaleString('vi-VN', {
            weekday: 'short', day: '2-digit', month: '2-digit', year: 'numeric',
            hour: '2-digit', minute: '2-digit', second: '2-digit'
        });
    }
}
setInterval(updateClock, 1000);
updateClock();

// Sidebar toggle
document.getElementById('sidebarToggle')?.addEventListener('click', function () {
    document.getElementById('sidebar').classList.toggle('open');
});

// Auto-dismiss alerts
document.querySelectorAll('.alert').forEach(alert => {
    setTimeout(() => {
        const bsAlert = new bootstrap.Alert(alert);
        bsAlert.close();
    }, 4000);
});

// Number format
function formatVND(num) {
    return new Intl.NumberFormat('vi-VN').format(Math.round(num)) + 'đ';
}

// Confirm delete modal
function confirmDelete(formId, name) {
    if (confirm(`Bạn có chắc muốn xóa "${name}"?\nHành động này không thể hoàn tác!`)) {
        document.getElementById(formId)?.submit();
    }
}

// ===============================================
// ĐƠN HÀNG - Bảng chi tiết AJAX
// ===============================================

let orderItems = [];

function initOrderTable() {
    renderOrderTable();
    updateOrderTotal();
}

function addOrderRow() {
    const maVT = document.getElementById('selectVatTu');
    const sl = document.getElementById('inputSoLuong');
    const dg = document.getElementById('inputDonGia');

    if (!maVT || !maVT.value) { alert('Vui lòng chọn vật tư!'); return; }
    if (!sl || sl.value < 1) { alert('Số lượng phải lớn hơn 0!'); return; }
    if (!dg || dg.value < 0) { alert('Đơn giá không hợp lệ!'); return; }

    const selectedOption = maVT.options[maVT.selectedIndex];
    const item = {
        maVatTu: parseInt(maVT.value),
        tenVatTu: selectedOption.dataset.ten,
        donVi: selectedOption.dataset.donvi,
        soLuong: parseInt(sl.value),
        donGia: parseFloat(dg.value),
        ghiChu: document.getElementById('inputGhiChu')?.value || ''
    };
    item.thanhTien = item.soLuong * item.donGia;

    // Kiểm tra trùng
    const existing = orderItems.findIndex(x => x.maVatTu === item.maVatTu);
    if (existing >= 0) {
        orderItems[existing].soLuong += item.soLuong;
        orderItems[existing].thanhTien = orderItems[existing].soLuong * orderItems[existing].donGia;
    } else {
        orderItems.push(item);
    }

    renderOrderTable();
    updateOrderTotal();

    // Reset inputs
    maVT.value = '';
    sl.value = '';
    dg.value = '';
    if (document.getElementById('inputGhiChu')) document.getElementById('inputGhiChu').value = '';
}

function removeOrderRow(idx) {
    orderItems.splice(idx, 1);
    renderOrderTable();
    updateOrderTotal();
}

function renderOrderTable() {
    const tbody = document.getElementById('orderTableBody');
    if (!tbody) return;

    if (orderItems.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center text-muted py-4">
            <i class="fas fa-inbox fa-2x mb-2 d-block opacity-25"></i>Chưa có vật tư nào
        </td></tr>`;
        return;
    }

    tbody.innerHTML = orderItems.map((item, idx) => `
        <tr>
            <td>${idx + 1}</td>
            <td>
                <div class="fw-600">${item.tenVatTu}</div>
                <input type="hidden" name="maVatTuList" value="${item.maVatTu}">
            </td>
            <td>
                <input type="number" class="form-control form-control-sm text-center" style="width:80px"
                    name="soLuongList" value="${item.soLuong}" min="1"
                    onchange="updateItem(${idx},'soLuong',this.value)">
            </td>
            <td>${item.donVi}</td>
            <td>
                <input type="number" class="form-control form-control-sm text-end" style="width:120px"
                    name="donGiaList" value="${item.donGia}" min="0"
                    onchange="updateItem(${idx},'donGia',this.value)">
            </td>
            <td class="text-end fw-600 text-primary">${new Intl.NumberFormat('vi-VN').format(item.thanhTien)}</td>
            <td>
                <button type="button" class="btn btn-danger btn-icon" onclick="removeOrderRow(${idx})">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

function updateItem(idx, field, value) {
    orderItems[idx][field] = parseFloat(value) || 0;
    orderItems[idx].thanhTien = orderItems[idx].soLuong * orderItems[idx].donGia;
    renderOrderTable();
    updateOrderTotal();
}

function updateOrderTotal() {
    const tongTien = orderItems.reduce((s, i) => s + i.thanhTien, 0);
    const ck = parseFloat(document.getElementById('inputChietKhau')?.value || 0);
    const tt = tongTien - ck;
    const dtt = parseFloat(document.getElementById('inputDaThanhToan')?.value || 0);
    const cl = tt - dtt;

    if (document.getElementById('displayTongTien')) {
        document.getElementById('displayTongTien').textContent = new Intl.NumberFormat('vi-VN').format(tongTien);
        document.getElementById('displayThanhTien').textContent = new Intl.NumberFormat('vi-VN').format(tt);
        document.getElementById('displayConLai').textContent = new Intl.NumberFormat('vi-VN').format(Math.max(0, cl));
    }

    // Hidden fields
    if (document.getElementById('hidTongTien')) {
        document.getElementById('hidTongTien').value = tongTien;
        document.getElementById('hidThanhTien').value = tt;
        document.getElementById('hidConLai').value = Math.max(0, cl);
    }
}

// Lấy giá bán khi chọn vật tư
async function onVatTuChange(select) {
    if (!select.value) return;
    try {
        const res = await fetch(`/VatTu/GetGiaBan?maVatTu=${select.value}`);
        const data = await res.json();
        if (document.getElementById('inputDonGia')) {
            document.getElementById('inputDonGia').value = data.giaBan;
        }
        if (document.getElementById('labelTonKho')) {
            document.getElementById('labelTonKho').textContent = `Tồn: ${data.ton} ${data.donVi}`;
        }
    } catch (e) { console.error(e); }
}