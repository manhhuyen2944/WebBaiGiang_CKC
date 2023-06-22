
var menuTogglek = document.querySelector('.menu-toggle');
var menuk = document.querySelector('.thanh');

menuTogglek.addEventListener('click', function () {
    menuk.style.display = menuk.style.display === 'block' ? 'none' : 'block';
});

var menuToggle = document.querySelector('.close-btn');
var menu = document.querySelector('.thanh');

menuToggle.addEventListener('click', function () {
    menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
});





const box1 = document.querySelector('.thanh');
const box2 = document.querySelector('.noidung');

const maxHeight = Math.max(box1.offsetHeight, box2.offsetHeight);

box1.style.height = maxHeight + 'px';
box2.style.height = maxHeight + 'px';
