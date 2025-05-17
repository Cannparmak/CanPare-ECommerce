// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// AJAX isteklerini ayırt etmek için headers ekle
$.ajaxSetup({
    headers: {
        'X-Requested-With': 'XMLHttpRequest'
    }
});

// Favorilere ekleme/çıkarma fonksiyonu (toggle)
function addToFavorites(productId, button) {
    // Butonun mevcut durumunu kontrol et
    const isInFavorites = $(button).find('i').hasClass('bi-heart-fill');
    
    // Ürün favorilerde ise kaldır, değilse ekle
    if (isInFavorites) {
        // Favorilerden çıkarma
        $.ajax({
            url: '/Favorites/RemoveFromFavorites',
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    // Başarılı bildirim göster
                    showNotification('Ürün favorilerinizden çıkarıldı!', 'success');
                    
                    // Kalp butonunu boş kalp yap
                    $(button).find('i').removeClass('bi-heart-fill text-danger').addClass('bi-heart');
                    
                    // Üstteki favori sayacını güncelle
                    updateFavoriteCountHeader(response.favoriteCount);
                    
                    // Animasyon ekle
                    $(button).addClass('favorite-removed');
                    setTimeout(function() {
                        $(button).removeClass('favorite-removed');
                    }, 1000);
                } else {
                    if (response.message && response.message.includes("giriş")) {
                        // Kullanıcı giriş yapmamışsa giriş sayfasına yönlendir
                        window.location.href = '/Account/SignIn';
                    } else {
                        showNotification(response.message || 'Bir hata oluştu!', 'danger');
                    }
                }
            },
            error: function () {
                showNotification('Bir hata oluştu!', 'danger');
            }
        });
    } else {
        // Favorilere ekleme
        $.ajax({
            url: '/Favorites/AddToFavorites',
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    // Başarılı bildirim göster
                    showNotification('Ürün favorilerinize eklendi!', 'success');
                    
                    // Kalp butonunu kırmızı yap
                    $(button).find('i').removeClass('bi-heart').addClass('bi-heart-fill text-danger');
                    
                    // Üstteki favori sayacını güncelle
                    updateFavoriteCountHeader(response.favoriteCount);
                    
                    // Animasyon ekle
                    $(button).addClass('favorite-added');
                    setTimeout(function() {
                        $(button).removeClass('favorite-added');
                    }, 1000);
                } else {
                    if (response.message && response.message.includes("giriş")) {
                        // Kullanıcı giriş yapmamışsa giriş sayfasına yönlendir
                        window.location.href = '/Account/SignIn';
                    } else {
                        showNotification(response.message || 'Bir hata oluştu!', 'danger');
                    }
                }
            },
            error: function () {
                showNotification('Bir hata oluştu!', 'danger');
            }
        });
    }
}

// Sepete ekleme fonksiyonu
function addToCart(productId, button, quantity = 1) {
    $.ajax({
        url: '/Cart/Add',
        type: 'POST',
        data: { productId: productId, quantity: quantity },
        success: function (response) {
            if (response.success) {
                showNotification('Ürün sepetinize eklendi!', 'success');
                
                // Sepet butonuna animasyon ekle
                $(button).addClass('cart-added');
                setTimeout(function() {
                    $(button).removeClass('cart-added');
                }, 1000);
                
                // Üstteki sepet sayacını güncelle
                updateCartCountHeader(response.cartCount);
            } else {
                showNotification(response.message, 'danger');
            }
        },
        error: function () {
            showNotification('Bir hata oluştu!', 'danger');
        }
    });
}

// Bildirim fonksiyonu
function showNotification(message, type) {
    // Varsa eski bildirimi kaldır
    $('.notification').remove();
    
    // Yeni bildirim oluştur
    var notification = $('<div class="notification notification-' + type + '">' + message + '</div>');
    $('body').append(notification);
    
    // Animasyonla göster
    setTimeout(function() {
        notification.addClass('show');
    }, 10);
    
    // 3 saniye sonra kaldır
    setTimeout(function() {
        notification.removeClass('show');
        setTimeout(function() {
            notification.remove();
        }, 300);
    }, 3000);
}

// Favorilerdeki sayıyı güncelle
function updateFavoriteCountHeader(count) {
    var badge = $('#favorite-count-badge');
    
    if (count > 0) {
        badge.text(count).show();
        $('#favorite-header-link i').removeClass('far').addClass('fas text-danger');
    } else {
        badge.text(0).hide();
        $('#favorite-header-link i').removeClass('fas text-danger').addClass('far');
    }
}

// Sepetteki sayıyı güncelle
function updateCartCountHeader(count) {
    // Sepet sayacı mantığı (sepet dropdown'ınıza bağlı olarak değişebilir)
}

// Ürünün favorilerde olup olmadığını kontrol et
function checkFavoriteStatus(productId, button) {
    $.ajax({
        url: '/Favorites/IsProductInFavorites',
        type: 'GET',
        data: { productId: productId },
        success: function (response) {
            if (response.isInFavorites) {
                $(button).find('i').removeClass('bi-heart').addClass('bi-heart-fill text-danger');
            } else {
                $(button).find('i').removeClass('bi-heart-fill text-danger').addClass('bi-heart');
            }
        }
    });
}

// Sayfa yüklendiğinde tüm ürünlerin favori durumunu kontrol et
$(document).ready(function() {
    // Tüm favori butonlarını kontrol et
    $('.favorite-button').each(function() {
        var productId = $(this).data('product-id');
        checkFavoriteStatus(productId, this);
    });
});

// CSS stillerini ekle
$("<style>")
    .prop("type", "text/css")
    .html(`
        .notification {
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 15px 25px;
            border-radius: 5px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            z-index: 9999;
            opacity: 0;
            transform: translateY(-20px);
            transition: opacity 0.3s, transform 0.3s;
        }
        
        .notification.show {
            opacity: 1;
            transform: translateY(0);
        }
        
        .notification-success {
            background-color: #28a745;
            color: white;
        }
        
        .notification-danger {
            background-color: #dc3545;
            color: white;
        }
        
        .favorite-added {
            animation: heartBeat 1s;
        }
        
        .favorite-removed {
            animation: heartShake 1s;
        }
        
        .cart-added {
            animation: bounce 1s;
        }
        
        @keyframes heartBeat {
            0% { transform: scale(1); }
            14% { transform: scale(1.3); }
            28% { transform: scale(1); }
            42% { transform: scale(1.3); }
            70% { transform: scale(1); }
        }
        
        @keyframes heartShake {
            0% { transform: rotate(0); }
            15% { transform: rotate(-15deg); }
            30% { transform: rotate(15deg); }
            45% { transform: rotate(-15deg); }
            60% { transform: rotate(15deg); }
            75% { transform: rotate(-15deg); }
            100% { transform: rotate(0); }
        }
        
        @keyframes bounce {
            0%, 20%, 50%, 80%, 100% { transform: translateY(0); }
            40% { transform: translateY(-10px); }
            60% { transform: translateY(-5px); }
        }
    `)
    .appendTo("head");
