(() => {
  const hero = document.querySelector('.hero-swiper');
  if (hero && window.Swiper) {
    new Swiper('.hero-swiper', {
      loop: true,
      effect: 'fade',
      autoplay: { delay: 5500, disableOnInteraction: false },
      pagination: { el: '.swiper-pagination', clickable: true },
      navigation: { nextEl: '.swiper-button-next', prevEl: '.swiper-button-prev' }
    });
  }

  const testimonials = document.querySelector('.testimonial-swiper');
  if (testimonials && window.Swiper) {
    new Swiper('.testimonial-swiper', {
      loop: true,
      autoplay: { delay: 4500 },
      slidesPerView: 1,
      spaceBetween: 18,
      breakpoints: {
        768: { slidesPerView: 2 },
        1200: { slidesPerView: 3 }
      }
    });
  }

  if (window.toastr) {
    toastr.options = {
      closeButton: true,
      progressBar: true,
      positionClass: 'toast-top-right',
      timeOut: 3500
    };
  }
})();
