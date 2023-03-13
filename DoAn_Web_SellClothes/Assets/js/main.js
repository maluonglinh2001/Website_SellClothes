//srcoll on top
let calcScrollValue = () => {
    let scrollProgress = document.getElementById("progress");
    let pos = document.documentElement.scrollTop;
    let calcHeight = document.documentElement.scrollHeight - document.documentElement.clientHeight;

    let scrollValue = Math.round((pos * 100) / calcHeight);

    if (pos > 100) {
        scrollProgress.style.display = "grid";
    } else {
        scrollProgress.style.display = "none";
    }
    scrollProgress.addEventListener("click", () => {
        document.documentElement.scrollTop = 0;
    });
    scrollProgress.style.background = `conic-gradient(#FFDFC0 ${scrollValue}%, #d7d7d7 ${scrollValue}%)`;
};

window.onscroll = calcScrollValue;

//scroll on top khi nhấn icon tìm kiếm
$(document).ready(function () {
    $('.search-icon').mousedown(function () {
        $('html').animate({
            scrollTop: 0
        }, 400);
        return false;
    });
});


//trạng thái nút ở thanh lọc sản phẩm theo loại
$('.men-btn').click(function () {
    $('.sidebar ul .men-show').toggleClass("show");
    $('.sidebar ul .first').toggleClass("rotate");
})
$('.women-btn').click(function () {
    $('.sidebar ul .women-show').toggleClass("show1");
    $('.sidebar ul .second').toggleClass("rotate");
})

$('.sidebar ul li').click(function () {
    $(this).addClass("active").siblings().removeClass("active");
})

//chuyển ảnh trong chi tiết sản phẩm
$(document).ready(function () {
    $('.list__img-small img').click(function () {
        $('.imgBox img').attr("src", $(this).attr("src"));
    })
})

//nút tăng giảm số lượng
const plus = document.querySelector('.plus'),
    minus = document.querySelector('.minus'),
    num = document.querySelector('.num');

let a = 1;

plus.addEventListener("click", () => {
    a++;
    a = (a < 10) ? "0" + a : a;
    num.innerText = a;
    $('#num').val($('.num').text());
});

minus.addEventListener("click", () => {
    if (a > 1) {
        a--;
        a = (a < 10) ? "0" + a : a;
        num.innerText = a;
        $('#num').val($('.num').text());
    }
});
//hết nút tăng giảm số lượng




