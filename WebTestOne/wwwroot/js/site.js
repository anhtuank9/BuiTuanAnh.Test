// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", () => {
    const toggle = document.querySelector("[data-site-nav-toggle]");
    const navigation = document.querySelector("[data-site-nav]");

    if (toggle && navigation) {
        toggle.addEventListener("click", () => {
            const isOpen = navigation.classList.toggle("is-open");
            toggle.setAttribute("aria-expanded", String(isOpen));
        });
    }

    document.querySelectorAll("[data-ad-carousel]").forEach((carousel) => {
        const slides = Array.from(carousel.querySelectorAll("[data-ad-slide]"));
        const dots = Array.from(carousel.querySelectorAll("[data-ad-dot]"));

        if (slides.length < 2 || window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
            return;
        }

        const interval = Number(carousel.dataset.interval) || 4500;
        let activeIndex = 0;
        let timer;

        const showSlide = (nextIndex) => {
            slides[activeIndex].classList.remove("is-active");
            slides[activeIndex].setAttribute("aria-hidden", "true");
            dots[activeIndex]?.classList.remove("is-active");

            activeIndex = nextIndex;
            slides[activeIndex].classList.add("is-active");
            slides[activeIndex].setAttribute("aria-hidden", "false");
            dots[activeIndex]?.classList.add("is-active");
        };

        const start = () => {
            window.clearInterval(timer);
            timer = window.setInterval(() => showSlide((activeIndex + 1) % slides.length), interval);
        };

        carousel.addEventListener("mouseenter", () => window.clearInterval(timer));
        carousel.addEventListener("mouseleave", start);
        carousel.addEventListener("focusin", () => window.clearInterval(timer));
        carousel.addEventListener("focusout", start);
        start();
    });
});
