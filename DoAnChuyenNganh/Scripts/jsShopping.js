$(function () {
    const productId = $('#product-data').data('product-id');
    let quantity = 1;

    // Cộng trừ số lượng
    $('.plus').click(() => {
        quantity++;
        $('#quantity_value').text(quantity);
    });

    $('.minus').click(() => {
        if (quantity > 1) {
            quantity--;
            $('#quantity_value').text(quantity);
        }
    });

    // Chọn màu
    $('.color-option').click(function () {
        $('.color-option').removeClass('active');
        $(this).addClass('active');
        $('#selectedColor').val($(this).data('color'));
        findVariant();
    });

    // Chọn size
    $('.size-option').click(function () {
        $('.size-option').removeClass('active');
        $(this).addClass('active');
        $('#selectedSize').val($(this).data('size'));
        findVariant();
    });

    function findVariant() {
        const color = ($('#selectedColor').val() || '').trim();
        const size = ($('#selectedSize').val() || '').trim();

        if (!color || !size) return;

        $.get('/GioHangs/FindVariant', { productId, color, size })
            .done(res => {
                if (res.success) {
                    $('#selectedVariantId').val(res.variantId);
                    console.log('✅ Found variant:', res.variantId);
                } else {
                    $('#selectedVariantId').val('');
                    alert('❌ Không tìm thấy biến thể với màu ' + color + ' và size ' + size);
                }
            })
            .fail(err => {
                console.error('FindVariant error', err);
            });
    }

    // Add to cart
    $('.btnAddToCart').click(function (e) {
        e.preventDefault();

        const variantId = $('#selectedVariantId').val();
        if (!variantId) {
            alert('Vui lòng chọn Màu sắc và Kích thước!');
            return;
        }

        $.post('/GioHangs/AddToCart', { id: variantId, quantity })
            .done(res => {
                if (res.success) {
                    alert(res.message);
                    $('#checkout_items').text(res.count);
                    $('.cart_count').text(res.count);
                } else {
                    alert(res.message || 'Thêm vào giỏ hàng thất bại');
                }
            })
            .fail(() => alert('Lỗi hệ thống'));
    });
});
