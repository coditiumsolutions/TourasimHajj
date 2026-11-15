(() => {
  const body = document.body;
  const sidebar = document.getElementById('adminSidebar');
  const backdrop = document.getElementById('sidebarBackdrop');
  const toggle = document.getElementById('sidebarToggle');
  const storageKey = 'tourismhajj.sidebarCollapsed';

  function isMobile() {
    return window.matchMedia('(max-width: 991.98px)').matches;
  }

  function setCollapsed(collapsed) {
    if (!sidebar) return;
    if (isMobile()) {
      sidebar.classList.toggle('mobile-open', collapsed === false ? sidebar.classList.contains('mobile-open') : false);
      return;
    }
    sidebar.classList.toggle('collapsed', collapsed);
    body.classList.toggle('sidebar-collapsed', collapsed);
    localStorage.setItem(storageKey, collapsed ? '1' : '0');
  }

  function closeMobileSidebar() {
    sidebar?.classList.remove('mobile-open');
    backdrop?.classList.remove('show');
  }

  function openMobileSidebar() {
    sidebar?.classList.add('mobile-open');
    backdrop?.classList.add('show');
  }

  if (toggle && sidebar) {
    const saved = localStorage.getItem(storageKey) === '1';
    if (!isMobile()) setCollapsed(saved);

    toggle.addEventListener('click', () => {
      if (isMobile()) {
        if (sidebar.classList.contains('mobile-open')) closeMobileSidebar();
        else openMobileSidebar();
      } else {
        setCollapsed(!sidebar.classList.contains('collapsed'));
      }
    });
  }

  backdrop?.addEventListener('click', closeMobileSidebar);
  window.addEventListener('resize', () => {
    if (!isMobile()) {
      closeMobileSidebar();
      setCollapsed(localStorage.getItem(storageKey) === '1');
    } else {
      sidebar?.classList.remove('collapsed');
      body.classList.remove('sidebar-collapsed');
    }
  });

  document.querySelectorAll('[data-menu-toggle]').forEach(btn => {
    btn.addEventListener('click', () => {
      const item = btn.closest('.menu-item');
      if (!item) return;
      const parent = item.parentElement;
      parent?.querySelectorAll('.menu-item.open').forEach(openItem => {
        if (openItem !== item) openItem.classList.remove('open');
      });
      item.classList.toggle('open');
    });
  });

  if (window.toastr) {
    toastr.options = {
      closeButton: true,
      progressBar: true,
      positionClass: 'toast-top-right',
      timeOut: 3500
    };
  }

  document.querySelectorAll('[data-confirm]').forEach(el => {
    el.addEventListener('click', function (e) {
      e.preventDefault();
      const message = this.getAttribute('data-confirm') || 'Are you sure?';
      const href = this.getAttribute('href');
      const form = this.closest('form');
      Swal.fire({
        title: 'Please confirm',
        text: message,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#0b1f3a',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Yes, continue'
      }).then(result => {
        if (!result.isConfirmed) return;
        if (form) form.submit();
        else if (href) window.location.href = href;
      });
    });
  });
})();
