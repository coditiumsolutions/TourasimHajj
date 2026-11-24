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

  // Module top-nav: show matching sublinks in the side navbar
  const moduleStorageKey = 'tourismhajj.activeModule';
  const nestedModules = ['hajj', 'umrah', 'supplier', 'accounts', 'reports', 'administration'];

  function setActiveModule(module) {
    if (!module) return;
    document.querySelectorAll('.admin-module-link[data-module]').forEach(btn => {
      btn.classList.toggle('active', btn.getAttribute('data-module') === module);
    });
    document.querySelectorAll('[data-module-panel]').forEach(panel => {
      panel.classList.toggle('active', panel.getAttribute('data-module-panel') === module);
    });
    document.querySelectorAll('[data-module-group]').forEach(group => {
      group.classList.toggle('active', nestedModules.includes(module));
      group.classList.remove('open');
    });
    localStorage.setItem(moduleStorageKey, module);
    if (isMobile()) openMobileSidebar();
  }

  document.querySelectorAll('[data-module]').forEach(btn => {
    btn.addEventListener('click', () => {
      const module = btn.getAttribute('data-module');
      setActiveModule(module);
    });
  });

  document.querySelectorAll('[data-module-group-toggle]').forEach(btn => {
    btn.addEventListener('click', (e) => {
      e.preventDefault();
      e.stopPropagation();
      const group = btn.closest('[data-module-group]');
      if (!group) return;
      document.querySelectorAll('[data-module-group].open').forEach(openGroup => {
        if (openGroup !== group) openGroup.classList.remove('open');
      });
      group.classList.toggle('open');
    });
  });

  document.addEventListener('click', (e) => {
    const target = e.target;
    if (!(target instanceof Element)) return;
    if (target.closest('[data-module-group]')) return;
    document.querySelectorAll('[data-module-group].open').forEach(group => group.classList.remove('open'));
  });

  const serverModule = document.querySelector('.admin-mainnav')?.getAttribute('data-active-module');
  const savedModule = localStorage.getItem(moduleStorageKey);
  const path = (location.pathname || '').toLowerCase();
  const onDashboard = path === '/dashboard' || path.endsWith('/dashboard/index') || path.endsWith('/dashboard');
  // Dashboard always shows Travel links (including packages). Elsewhere prefer route/server module.
  setActiveModule(onDashboard ? 'travel' : (serverModule || savedModule || 'travel'));

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
