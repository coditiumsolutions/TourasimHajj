(function () {
  const body = document.getElementById('groupMembersBody');
  const template = document.getElementById('groupMemberRowTemplate');
  const addBtn = document.getElementById('addGroupMemberBtn');
  if (!body || !template || !addBtn) return;

  function reindexRows() {
    Array.from(body.querySelectorAll('.group-member-row')).forEach((row, index) => {
      row.querySelectorAll('[name]').forEach((input) => {
        const name = input.getAttribute('name');
        if (!name) return;
        input.setAttribute('name', name.replace(/Members\[\d+\]/, `Members[${index}]`));
      });
    });
  }

  function ensureOneRow() {
    if (body.querySelectorAll('.group-member-row').length === 0) {
      addRow();
    }
  }

  function addRow() {
    const html = template.innerHTML.replaceAll('__INDEX__', String(body.querySelectorAll('.group-member-row').length));
    body.insertAdjacentHTML('beforeend', html);
    reindexRows();
  }

  addBtn.addEventListener('click', () => addRow());

  body.addEventListener('click', (e) => {
    const btn = e.target.closest('.remove-member-btn');
    if (!btn) return;
    const row = btn.closest('.group-member-row');
    if (!row) return;
    row.remove();
    reindexRows();
    ensureOneRow();
  });
})();
