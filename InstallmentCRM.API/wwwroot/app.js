/* =======================================================================
   InstallmentCRM · "Гроссбух" front end
   Vanilla JS SPA talking to the ASP.NET Core API on the same origin.
   ======================================================================= */

const API_BASE = ""; // same-origin, served from wwwroot by the API itself
document.getElementById("api-base-label").textContent =
  window.location.origin + "/api";

/* ----------------------------- API client ----------------------------- */

const Auth = {
  get token() { return localStorage.getItem("crm_token"); },
  set token(v) { v ? localStorage.setItem("crm_token", v) : localStorage.removeItem("crm_token"); },

  get claims() {
    const t = this.token;
    if (!t) return null;
    try {
      const payload = t.split(".")[1];
      const json = decodeURIComponent(
        atob(payload.replace(/-/g, "+").replace(/_/g, "/"))
          .split("")
          .map((c) => "%" + c.charCodeAt(0).toString(16).padStart(2, "0"))
          .join("")
      );
      return JSON.parse(json);
    } catch {
      return null;
    }
  },

  get email() {
    const c = this.claims;
    if (!c) return null;
    return c["email"] || c["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || null;
  },

  get roles() {
    const c = this.claims;
    if (!c) return [];
    const key = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    const val = c[key];
    if (!val) return [];
    return Array.isArray(val) ? val : [val];
  },

  hasRole(...roles) {
    return this.roles.some((r) => roles.includes(r));
  },

  isExpired() {
    const c = this.claims;
    if (!c || !c.exp) return true;
    return Date.now() >= c.exp * 1000;
  },

  logout() {
    this.token = null;
  }
};

class ApiError extends Error {
  constructor(status, message, errors) {
    super(message);
    this.status = status;
    this.errors = errors || null;
  }
}

async function api(path, { method = "GET", body, auth = true } = {}) {
  const headers = { "Content-Type": "application/json" };
  if (auth && Auth.token) headers["Authorization"] = `Bearer ${Auth.token}`;

  let res;
  try {
    res = await fetch(`${API_BASE}/api${path}`, {
      method,
      headers,
      body: body !== undefined ? JSON.stringify(body) : undefined
    });
  } catch (networkErr) {
    throw new ApiError(0, "Не удается связаться с сервером. Проверьте, что API запущен и доступен.");
  }

  if (res.status === 204) return null;

  let data = null;
  const text = await res.text();
  if (text) {
    try { data = JSON.parse(text); } catch { /* non-json body */ }
  }

  if (!res.ok) {
    if (res.status === 401) {
      Auth.logout();
      showAuthScreen();
    }
    const message = (data && data.Message) || (data && data.message) || `Ошибка ${res.status}`;
    const errors = (data && (data.Errors || data.errors)) || null;
    throw new ApiError(res.status, message, errors);
  }

  return data;
}

/* ------------------------------ Helpers -------------------------------- */

function money(v) {
  const n = Number(v || 0);
  return n.toLocaleString("ru-RU", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

function dateStr(v) {
  if (!v) return "—";
  const d = new Date(v);
  if (isNaN(d)) return "—";
  return d.toLocaleDateString("ru-RU");
}

function daysWord(n) {
  const abs = Math.abs(n) % 100;
  const last = abs % 10;
  if (abs > 10 && abs < 20) return "дней";
  if (last === 1) return "день";
  if (last >= 2 && last <= 4) return "дня";
  return "дней";
}

function toast(message, type = "success") {
  const el = document.getElementById("toast");
  el.textContent = message;
  el.className = `toast ${type}`;
  clearTimeout(toast._t);
  toast._t = setTimeout(() => el.classList.add("hidden"), 3800);
}

function esc(s) {
  return String(s ?? "").replace(/[&<>"']/g, (c) => ({
    "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
  }[c]));
}

function statusStamp(status) {
  const map = {
    Active: ["stamp-active", "Активен"],
    Completed: ["stamp-completed", "Погашен"],
    Overdue: ["stamp-overdue", "Просрочен"],
    Cancelled: ["stamp-cancelled", "Отменён"]
  };
  const [cls, label] = map[status] || ["stamp-cancelled", status || "—"];
  return `<span class="stamp ${cls}">${label}</span>`;
}

/* ------------------------------- Modal ---------------------------------- */

const Modal = {
  el: document.getElementById("modal-backdrop"),
  title: document.getElementById("modal-title"),
  body: document.getElementById("modal-body"),

  open(title, bodyHtml) {
    this.title.textContent = title;
    this.body.innerHTML = bodyHtml;
    this.el.classList.remove("hidden");
  },
  close() {
    this.el.classList.add("hidden");
    this.body.innerHTML = "";
  }
};
document.getElementById("modal-close").addEventListener("click", () => Modal.close());
Modal.el.addEventListener("click", (e) => { if (e.target === Modal.el) Modal.close(); });

/* --------------------------- Auth screen -------------------------------- */

const authScreen = document.getElementById("auth-screen");
const mainScreen = document.getElementById("main-screen");

function showAuthScreen() {
  authScreen.classList.remove("hidden");
  mainScreen.classList.add("hidden");
}
function showMainScreen() {
  authScreen.classList.add("hidden");
  mainScreen.classList.remove("hidden");
  applyNavVisibility();
  renderUserChip();
  navigateTo(currentView || "dashboard");
}

document.querySelectorAll(".auth-tab").forEach((tab) => {
  tab.addEventListener("click", () => {
    document.querySelectorAll(".auth-tab").forEach((t) => t.classList.remove("active"));
    tab.classList.add("active");
    document.getElementById("login-form").classList.toggle("hidden", tab.dataset.tab !== "login");
    document.getElementById("register-form").classList.toggle("hidden", tab.dataset.tab !== "register");
  });
});

document.getElementById("login-form").addEventListener("submit", async (e) => {
  e.preventDefault();
  const errEl = document.querySelector('.form-error[data-for="login"]');
  errEl.textContent = "";
  const fd = new FormData(e.target);
  try {
    const res = await api("/auth/login", {
      method: "POST",
      auth: false,
      body: { email: fd.get("email"), password: fd.get("password") }
    });
    Auth.token = res.token || res.Token;
    showMainScreen();
  } catch (err) {
    errEl.textContent = err.message;
  }
});

document.getElementById("register-form").addEventListener("submit", async (e) => {
  e.preventDefault();
  const errEl = document.querySelector('.form-error[data-for="register"]');
  errEl.textContent = "";
  const fd = new FormData(e.target);
  try {
    await api("/auth/register", {
      method: "POST",
      auth: false,
      body: {
        fullName: fd.get("fullName"),
        email: fd.get("email"),
        password: fd.get("password"),
        role: fd.get("role")
      }
    });
    toast("Регистрация выполнена. Теперь войдите.", "success");
    document.querySelector('.auth-tab[data-tab="login"]').click();
    e.target.reset();
  } catch (err) {
    if (err.errors) {
      errEl.textContent = Object.values(err.errors).flat().join(" ");
    } else {
      errEl.textContent = err.message;
    }
  }
});

document.getElementById("logout-btn").addEventListener("click", () => {
  Auth.logout();
  showAuthScreen();
});

function renderUserChip() {
  const email = Auth.email || "—";
  document.getElementById("user-email").textContent = email;
  document.getElementById("user-role").textContent = Auth.roles.join(", ") || "—";
  document.getElementById("user-avatar").textContent = email.charAt(0).toUpperCase();
}

/* ------------------------------ Navigation ------------------------------- */

const viewTitles = {
  dashboard: "Сводка",
  contracts: "Договоры рассрочки",
  payments: "Платежи",
  customers: "Клиенты",
  products: "Товары",
  categories: "Категории"
};

let currentView = "dashboard";

document.querySelectorAll(".nav-item").forEach((btn) => {
  btn.addEventListener("click", () => {
    navigateTo(btn.dataset.view);
    closeMobileMenu();
  });
});

/* ------------------------- Mobile sidebar toggle ------------------------- */

const sidebarEl = document.querySelector(".sidebar");
const sidebarOverlay = document.getElementById("sidebar-overlay");
const mobileMenuToggle = document.getElementById("mobile-menu-toggle");

function openMobileMenu() {
  sidebarEl.classList.add("open");
  sidebarOverlay.classList.remove("hidden");
}
function closeMobileMenu() {
  sidebarEl.classList.remove("open");
  sidebarOverlay.classList.add("hidden");
}
mobileMenuToggle.addEventListener("click", () => {
  sidebarEl.classList.contains("open") ? closeMobileMenu() : openMobileMenu();
});
sidebarOverlay.addEventListener("click", closeMobileMenu);

function navigateTo(view) {
  currentView = view;
  document.querySelectorAll(".nav-item").forEach((b) => b.classList.toggle("active", b.dataset.view === view));
  document.getElementById("view-title").textContent = viewTitles[view] || view;
  document.getElementById("view-actions").innerHTML = "";
  const root = document.getElementById("view-root");
  root.innerHTML = `<div class="loading-row">Загрузка…</div>`;

  const renderers = {
    dashboard: renderDashboard,
    contracts: renderContracts,
    payments: renderPayments,
    customers: renderCustomers,
    products: renderProducts,
    categories: renderCategories
  };
  (renderers[view] || renderDashboard)().catch((err) => {
    root.innerHTML = `<div class="loading-row">${esc(err.message)}</div>`;
  });
}

/* Hide nav items the current role has no server access to (best-effort UX;
   the API still enforces this authoritatively). */
function applyNavVisibility() {
  const canWrite = Auth.hasRole("Manager", "Cashier");
  const canCategories = Auth.hasRole("Manager");
  document.querySelector('.nav-item[data-view="contracts"]').classList.toggle("hidden", !canWrite);
  document.querySelector('.nav-item[data-view="payments"]').classList.toggle("hidden", !canWrite);
  document.querySelector('.nav-item[data-view="customers"]').classList.toggle("hidden", !canWrite);
  document.querySelector('.nav-item[data-view="products"]').classList.toggle("hidden", !canWrite);
  document.querySelector('.nav-item[data-view="categories"]').classList.toggle("hidden", !canCategories);
}

/* ------------------------------ Dashboard -------------------------------- */

async function renderDashboard() {
  const [summary, debtors, upcoming] = await Promise.all([
    api("/dashboard"),
    api("/dashboard/top-debtors"),
    api("/dashboard/upcoming-payments")
  ]);

  const root = document.getElementById("view-root");
  root.innerHTML = `
    <div class="kpi-grid">
      ${kpi("Клиентов", summary.totalCustomers ?? summary.TotalCustomers)}
      ${kpi("Товаров", summary.totalProducts ?? summary.TotalProducts)}
      ${kpi("Договоров всего", summary.totalContracts ?? summary.TotalContracts)}
      ${kpi("Активных договоров", summary.activeContracts ?? summary.ActiveContracts, "accent-blue")}
      ${kpi("Погашено договоров", summary.completedContracts ?? summary.CompletedContracts, "accent-green")}
      ${kpi("Сумма договоров", money(summary.totalContractAmount ?? summary.TotalContractAmount), "accent-gold")}
      ${kpi("Оплачено", money(summary.totalPaidAmount ?? summary.TotalPaidAmount), "accent-green")}
      ${kpi("Остаток долга", money(summary.remainingAmount ?? summary.RemainingAmount), "accent-red")}
      ${kpi("Платежей просрочено", summary.overdueSchedules ?? summary.OverdueSchedules, "accent-red")}
      ${kpi("Платежей сегодня", summary.dueTodaySchedules ?? summary.DueTodaySchedules, "accent-gold")}
    </div>

    <div class="section-block">
      <h3>Топ должников</h3>
      <div class="table-wrap">
        <table>
          <thead><tr>
            <th>Клиент</th><th>Телефон</th><th>Договор</th>
            <th>Долг</th><th>Просрочек</th><th>Дней просрочки</th>
          </tr></thead>
          <tbody>
            ${debtors.length ? debtors.map((d) => `
              <tr>
                <td>${esc(d.customerName ?? d.CustomerName)}</td>
                <td class="num">${esc(d.phoneNumber ?? d.PhoneNumber)}</td>
                <td>${esc(d.contractNumber ?? d.ContractNumber)}</td>
                <td class="num">${money(d.remainingDebt ?? d.RemainingDebt)}</td>
                <td class="num">${d.overduePayments ?? d.OverduePayments}</td>
                <td class="num">${d.daysLate ?? d.DaysLate}</td>
              </tr>`).join("") : emptyRow(6, "Должников нет — прекрасная работа.")}
          </tbody>
        </table>
      </div>
    </div>

    <div class="section-block">
      <h3>Ближайшие платежи (7 дней)</h3>
      <div class="table-wrap">
        <table>
          <thead><tr>
            <th>Клиент</th><th>Телефон</th><th>Договор</th>
            <th>Сумма</th><th>Срок</th><th>Осталось дней</th>
          </tr></thead>
          <tbody>
            ${upcoming.length ? upcoming.map((u) => `
              <tr>
                <td>${esc(u.customerName ?? u.CustomerName)}</td>
                <td class="num">${esc(u.phoneNumber ?? u.PhoneNumber)}</td>
                <td>${esc(u.contractNumber ?? u.ContractNumber)}</td>
                <td class="num">${money(u.amount ?? u.Amount)}</td>
                <td class="num">${dateStr(u.dueDate ?? u.DueDate)}</td>
                <td class="num">${u.daysLeft ?? u.DaysLeft}</td>
              </tr>`).join("") : emptyRow(6, "Ближайших платежей нет.")}
          </tbody>
        </table>
      </div>
    </div>
  `;
}

function kpi(label, value, accent = "") {
  return `<div class="kpi-card"><div class="kpi-label">${esc(label)}</div><div class="kpi-value ${accent}">${esc(value)}</div></div>`;
}
function emptyRow(cols, text) {
  return `<tr class="empty-row"><td colspan="${cols}">${esc(text)}</td></tr>`;
}

/* ------------------------------ Categories -------------------------------- */

async function renderCategories() {
  const canManage = Auth.hasRole("Manager");
  const actions = document.getElementById("view-actions");
  actions.innerHTML = canManage ? `<button class="btn btn-primary btn-small" id="add-category">+ Категория</button>` : "";
  if (canManage) document.getElementById("add-category").addEventListener("click", () => openCategoryModal());

  const categories = await api("/categories");
  const root = document.getElementById("view-root");
  root.innerHTML = `
    <div class="table-wrap">
      <table>
        <thead><tr><th>Название</th><th></th></tr></thead>
        <tbody>
          ${categories.length ? categories.map((c) => `
            <tr>
              <td>${esc(c.name ?? c.Name)}</td>
              <td class="row-actions">
                ${canManage ? `
                  <button class="btn btn-ghost btn-small" data-edit="${c.id ?? c.Id}">Изменить</button>
                  <button class="btn btn-danger btn-small" data-delete="${c.id ?? c.Id}">Удалить</button>
                ` : ""}
              </td>
            </tr>`).join("") : emptyRow(2, "Категорий пока нет.")}
        </tbody>
      </table>
    </div>
  `;

  root.querySelectorAll("[data-edit]").forEach((b) => b.addEventListener("click", () => {
    const cat = categories.find((c) => (c.id ?? c.Id) === b.dataset.edit);
    openCategoryModal(cat);
  }));
  root.querySelectorAll("[data-delete]").forEach((b) => b.addEventListener("click", () => confirmDelete(
    "Удалить категорию?", async () => {
      await api(`/categories/${b.dataset.delete}`, { method: "DELETE" });
      toast("Категория удалена.");
      navigateTo("categories");
    })));
}

function openCategoryModal(cat) {
  const isEdit = !!cat;
  Modal.open(isEdit ? "Изменить категорию" : "Новая категория", `
    <form id="category-form">
      <label>Название
        <input type="text" name="name" required value="${esc(cat ? (cat.name ?? cat.Name) : "")}" />
      </label>
      <p class="form-error" data-for="modal"></p>
      <div class="modal-foot">
        <button type="button" class="btn btn-ghost" id="cancel-modal">Отмена</button>
        <button type="submit" class="btn btn-primary">Сохранить</button>
      </div>
    </form>
  `);
  document.getElementById("cancel-modal").addEventListener("click", () => Modal.close());
  document.getElementById("category-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const name = new FormData(e.target).get("name");
    try {
      if (isEdit) {
        await api(`/categories/${cat.id ?? cat.Id}`, { method: "PUT", body: { id: cat.id ?? cat.Id, name } });
      } else {
        await api("/categories", { method: "POST", body: { name } });
      }
      Modal.close();
      toast(isEdit ? "Категория обновлена." : "Категория создана.");
      navigateTo("categories");
    } catch (err) {
      document.querySelector('.form-error[data-for="modal"]').textContent = err.message;
    }
  });
}

/* -------------------------------- Products --------------------------------- */

async function renderProducts() {
  const canManage = Auth.hasRole("Manager");
  const [products, categories] = await Promise.all([api("/products"), api("/categories").catch(() => [])]);

  const actions = document.getElementById("view-actions");
  actions.innerHTML = canManage ? `<button class="btn btn-primary btn-small" id="add-product">+ Товар</button>` : "";
  if (canManage) document.getElementById("add-product").addEventListener("click", () => openProductModal(null, categories));

  const root = document.getElementById("view-root");
  root.innerHTML = `
    <div class="table-wrap">
      <table>
        <thead><tr><th>Товар</th><th>Категория</th><th>Цена</th><th>Остаток</th><th></th></tr></thead>
        <tbody>
          ${products.length ? products.map((p) => `
            <tr>
              <td>${esc(p.name ?? p.Name)}</td>
              <td>${esc(p.categoryName ?? p.CategoryName)}</td>
              <td class="num">${money(p.price ?? p.Price)}</td>
              <td class="num">${p.quantity ?? p.Quantity}</td>
              <td class="row-actions">
                ${canManage ? `
                  <button class="btn btn-ghost btn-small" data-edit="${p.id ?? p.Id}">Изменить</button>
                  <button class="btn btn-danger btn-small" data-delete="${p.id ?? p.Id}">Удалить</button>
                ` : ""}
              </td>
            </tr>`).join("") : emptyRow(5, "Товаров пока нет.")}
        </tbody>
      </table>
    </div>
  `;

  root.querySelectorAll("[data-edit]").forEach((b) => b.addEventListener("click", () => {
    const p = products.find((x) => (x.id ?? x.Id) === b.dataset.edit);
    openProductModal(p, categories);
  }));
  root.querySelectorAll("[data-delete]").forEach((b) => b.addEventListener("click", () => confirmDelete(
    "Удалить товар?", async () => {
      await api(`/products/${b.dataset.delete}`, { method: "DELETE" });
      toast("Товар удален.");
      navigateTo("products");
    })));
}

function openProductModal(p, categories) {
  const isEdit = !!p;
  const catOptions = categories.map((c) => {
    const id = c.id ?? c.Id, name = c.name ?? c.Name;
    const selected = p && (p.categoryId ?? p.CategoryId) === id ? "selected" : "";
    return `<option value="${id}" ${selected}>${esc(name)}</option>`;
  }).join("");

  Modal.open(isEdit ? "Изменить товар" : "Новый товар", `
    <form id="product-form">
      <label>Название
        <input type="text" name="name" required value="${esc(p ? (p.name ?? p.Name) : "")}" />
      </label>
      <div class="form-row">
        <label>Цена
          <input type="number" name="price" min="0.01" step="0.01" required value="${p ? (p.price ?? p.Price) : ""}" />
        </label>
        <label>Остаток
          <input type="number" name="quantity" min="0" step="1" required value="${p ? (p.quantity ?? p.Quantity) : "0"}" />
        </label>
      </div>
      <label>Категория
        <select name="categoryId" required>${catOptions || '<option value="">Нет категорий</option>'}</select>
      </label>
      <label>Описание
        <textarea name="description">${esc(p ? (p.description ?? p.Description) : "")}</textarea>
      </label>
      <p class="form-error" data-for="modal"></p>
      <div class="modal-foot">
        <button type="button" class="btn btn-ghost" id="cancel-modal">Отмена</button>
        <button type="submit" class="btn btn-primary">Сохранить</button>
      </div>
    </form>
  `);
  document.getElementById("cancel-modal").addEventListener("click", () => Modal.close());
  document.getElementById("product-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const fd = new FormData(e.target);
    const payload = {
      name: fd.get("name"),
      price: Number(fd.get("price")),
      quantity: Number(fd.get("quantity")),
      description: fd.get("description") || "",
      categoryId: fd.get("categoryId")
    };
    try {
      if (isEdit) {
        payload.id = p.id ?? p.Id;
        await api(`/products/${payload.id}`, { method: "PUT", body: payload });
      } else {
        await api("/products", { method: "POST", body: payload });
      }
      Modal.close();
      toast(isEdit ? "Товар обновлен." : "Товар создан.");
      navigateTo("products");
    } catch (err) {
      document.querySelector('.form-error[data-for="modal"]').textContent = err.message;
    }
  });
}

/* -------------------------------- Customers --------------------------------- */

async function renderCustomers() {
  const canWrite = Auth.hasRole("Manager", "Cashier");
  const canDelete = Auth.hasRole("Manager");
  const actions = document.getElementById("view-actions");
  actions.innerHTML = canWrite ? `<button class="btn btn-primary btn-small" id="add-customer">+ Клиент</button>` : "";
  if (canWrite) document.getElementById("add-customer").addEventListener("click", () => openCustomerModal());

  const customers = await api("/customers");
  const root = document.getElementById("view-root");
  root.innerHTML = `
    <div class="table-wrap">
      <table>
        <thead><tr><th>Имя</th><th>Телефон</th><th>Паспорт</th><th></th></tr></thead>
        <tbody>
          ${customers.length ? customers.map((c) => `
            <tr>
              <td>${esc((c.firstName ?? c.FirstName) + " " + (c.lastName ?? c.LastName))}</td>
              <td class="num">${esc(c.phoneNumber ?? c.PhoneNumber)}</td>
              <td class="num">${esc(c.passportNumber ?? c.PassportNumber)}</td>
              <td class="row-actions">
                ${canWrite ? `<button class="btn btn-ghost btn-small" data-edit="${c.id ?? c.Id}">Изменить</button>` : ""}
                ${canDelete ? `<button class="btn btn-danger btn-small" data-delete="${c.id ?? c.Id}">Удалить</button>` : ""}
              </td>
            </tr>`).join("") : emptyRow(4, "Клиентов пока нет.")}
        </tbody>
      </table>
    </div>
  `;

  root.querySelectorAll("[data-edit]").forEach((b) => b.addEventListener("click", () => {
    const c = customers.find((x) => (x.id ?? x.Id) === b.dataset.edit);
    openCustomerModal(c);
  }));
  root.querySelectorAll("[data-delete]").forEach((b) => b.addEventListener("click", () => confirmDelete(
    "Удалить клиента?", async () => {
      await api(`/customers/${b.dataset.delete}`, { method: "DELETE" });
      toast("Клиент удален.");
      navigateTo("customers");
    })));
}

function openCustomerModal(c) {
  const isEdit = !!c;
  Modal.open(isEdit ? "Изменить клиента" : "Новый клиент", `
    <form id="customer-form">
      <div class="form-row">
        <label>Имя
          <input type="text" name="firstName" required value="${esc(c ? (c.firstName ?? c.FirstName) : "")}" />
        </label>
        <label>Фамилия
          <input type="text" name="lastName" required value="${esc(c ? (c.lastName ?? c.LastName) : "")}" />
        </label>
      </div>
      <label>Телефон <span class="field-hint">формат: +998901234567</span>
        <input type="text" name="phoneNumber" required value="${esc(c ? (c.phoneNumber ?? c.PhoneNumber) : "")}" />
      </label>
      <label>Номер паспорта
        <input type="text" name="passportNumber" required value="${esc(c ? (c.passportNumber ?? c.PassportNumber) : "")}" />
      </label>
      <p class="form-error" data-for="modal"></p>
      <div class="modal-foot">
        <button type="button" class="btn btn-ghost" id="cancel-modal">Отмена</button>
        <button type="submit" class="btn btn-primary">Сохранить</button>
      </div>
    </form>
  `);
  document.getElementById("cancel-modal").addEventListener("click", () => Modal.close());
  document.getElementById("customer-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const fd = new FormData(e.target);
    const payload = {
      firstName: fd.get("firstName"),
      lastName: fd.get("lastName"),
      phoneNumber: fd.get("phoneNumber"),
      passportNumber: fd.get("passportNumber")
    };
    try {
      if (isEdit) {
        payload.id = c.id ?? c.Id;
        await api(`/customers/${payload.id}`, { method: "PUT", body: payload });
      } else {
        await api("/customers", { method: "POST", body: payload });
      }
      Modal.close();
      toast(isEdit ? "Клиент обновлен." : "Клиент создан.");
      navigateTo("customers");
    } catch (err) {
      document.querySelector('.form-error[data-for="modal"]').textContent = err.message;
    }
  });
}

/* -------------------------------- Contracts --------------------------------- */

async function renderContracts() {
  const canWrite = Auth.hasRole("Manager", "Cashier");
  const canDelete = Auth.hasRole("Manager");
  const actions = document.getElementById("view-actions");
  actions.innerHTML = canWrite ? `<button class="btn btn-primary btn-small" id="add-contract">+ Договор</button>` : "";

  const contracts = await api("/installmentcontracts");
  const root = document.getElementById("view-root");
  root.innerHTML = `
    <div class="table-wrap">
      <table>
        <thead><tr>
          <th>№ Договора</th><th>Клиент</th><th>Товар</th><th>Сумма</th>
          <th>Ежемесячно</th><th>Статус</th><th></th>
        </tr></thead>
        <tbody>
          ${contracts.length ? contracts.map((c) => `
            <tr>
              <td class="num">${esc(c.contractNumber ?? c.ContractNumber)}</td>
              <td>${esc(c.customerName ?? c.CustomerName)}</td>
              <td>${esc(c.productName ?? c.ProductName)}</td>
              <td class="num">${money(c.totalAmount ?? c.TotalAmount)}</td>
              <td class="num">${money(c.monthlyPayment ?? c.MonthlyPayment)}</td>
              <td>${statusStamp(c.status ?? c.Status)}</td>
              <td class="row-actions">
                <button class="btn btn-ghost btn-small" data-view="${c.id ?? c.Id}">График</button>
                ${canDelete ? `<button class="btn btn-danger btn-small" data-delete="${c.id ?? c.Id}">Удалить</button>` : ""}
              </td>
            </tr>`).join("") : emptyRow(7, "Договоров пока нет.")}
        </tbody>
      </table>
    </div>
  `;

  if (canWrite) {
    document.getElementById("add-contract").addEventListener("click", async () => {
      const [customers, products] = await Promise.all([api("/customers"), api("/products")]);
      openContractModal(customers, products);
    });
  }

  root.querySelectorAll("[data-view]").forEach((b) => b.addEventListener("click", () => openContractDetail(b.dataset.view)));
  root.querySelectorAll("[data-delete]").forEach((b) => b.addEventListener("click", () => confirmDelete(
    "Удалить договор? Возможно только если по нему нет платежей.", async () => {
      await api(`/installmentcontracts/${b.dataset.delete}`, { method: "DELETE" });
      toast("Договор удален.");
      navigateTo("contracts");
    })));
}

function computeSchedulePreview({ price, downPayment, interestRate, months }) {
  const remaining = price - downPayment;
  const interest = (remaining * interestRate) / 100;
  const total = remaining + interest;
  const monthly = Math.round((total / months) * 100) / 100;
  const rows = [];
  for (let m = 1; m <= months; m++) {
    const amount = m === months ? Math.round((total - monthly * (months - 1)) * 100) / 100 : monthly;
    rows.push({ month: m, amount });
  }
  return { total, monthly, rows };
}

function openContractModal(customers, products) {
  const custOptions = customers.map((c) => `<option value="${c.id ?? c.Id}">${esc((c.firstName ?? c.FirstName) + " " + (c.lastName ?? c.LastName))}</option>`).join("");
  const prodOptions = products.map((p) => `<option value="${p.id ?? p.Id}" data-price="${p.price ?? p.Price}">${esc(p.name ?? p.Name)} — ${money(p.price ?? p.Price)}</option>`).join("");

  Modal.open("Новый договор рассрочки", `
    <form id="contract-form">
      <label>Клиент
        <select name="customerId" required>${custOptions || '<option value="">Нет клиентов</option>'}</select>
      </label>
      <label>Товар
        <select name="productId" id="contract-product" required>${prodOptions || '<option value="">Нет товаров</option>'}</select>
      </label>
      <div class="form-row">
        <label>Первый взнос
          <input type="number" name="downPayment" min="0" step="0.01" value="0" required />
        </label>
        <label>Ставка, % годовых
          <input type="number" name="interestRate" min="0" max="100" step="0.1" value="0" required />
        </label>
      </div>
      <div class="form-row">
        <label>Срок, мес.
          <input type="number" name="months" min="1" max="60" value="6" required />
        </label>
        <label>Дата начала
          <input type="date" name="startDate" required value="${new Date().toISOString().slice(0,10)}" />
        </label>
      </div>
      <label>Примечание
        <textarea name="notes"></textarea>
      </label>
      <div>
        <div class="field-hint" style="margin-bottom:0.4rem">Предварительный график:</div>
        <div class="schedule-preview" id="schedule-preview"></div>
      </div>
      <p class="form-error" data-for="modal"></p>
      <div class="modal-foot">
        <button type="button" class="btn btn-ghost" id="cancel-modal">Отмена</button>
        <button type="submit" class="btn btn-primary">Создать договор</button>
      </div>
    </form>
  `);

  const form = document.getElementById("contract-form");
  const updatePreview = () => {
    const fd = new FormData(form);
    const productSel = form.querySelector("#contract-product");
    const priceAttr = productSel.selectedOptions[0]?.dataset.price;
    if (!priceAttr) return;
    const { total, monthly, rows } = computeSchedulePreview({
      price: Number(priceAttr),
      downPayment: Number(fd.get("downPayment") || 0),
      interestRate: Number(fd.get("interestRate") || 0),
      months: Number(fd.get("months") || 1)
    });
    document.getElementById("schedule-preview").innerHTML =
      `<div><strong>Итого:</strong><span>${money(total)}</span></div>` +
      `<div><strong>Платеж/мес:</strong><span>${money(monthly)}</span></div>` +
      rows.slice(0, 12).map((r) => `<div><span>Месяц ${r.month}</span><span>${money(r.amount)}</span></div>`).join("") +
      (rows.length > 12 ? `<div class="muted">…и еще ${rows.length - 12} платежей</div>` : "");
  };
  form.addEventListener("input", updatePreview);
  updatePreview();

  document.getElementById("cancel-modal").addEventListener("click", () => Modal.close());
  form.addEventListener("submit", async (e) => {
    e.preventDefault();
    const fd = new FormData(form);
    const payload = {
      customerId: fd.get("customerId"),
      productId: fd.get("productId"),
      downPayment: Number(fd.get("downPayment")),
      interestRate: Number(fd.get("interestRate")),
      months: Number(fd.get("months")),
      startDate: fd.get("startDate"),
      notes: fd.get("notes") || null
    };
    try {
      await api("/installmentcontracts", { method: "POST", body: payload });
      Modal.close();
      toast("Договор создан.");
      navigateTo("contracts");
    } catch (err) {
      document.querySelector('.form-error[data-for="modal"]').textContent =
        err.errors ? Object.values(err.errors).flat().join(" ") : err.message;
    }
  });
}

async function openContractDetail(id) {
  const contract = await api(`/installmentcontracts/${id}`);
  const schedules = contract.paymentSchedules ?? contract.PaymentSchedules ?? [];
  const canWrite = Auth.hasRole("Manager", "Cashier");

  Modal.open(`Договор ${esc(contract.contractNumber ?? contract.ContractNumber)}`, `
    <div class="kpi-grid" style="grid-template-columns:repeat(2,1fr);margin-bottom:1rem">
      ${kpi("Клиент", contract.customerName ?? contract.CustomerName)}
      ${kpi("Товар", contract.productName ?? contract.ProductName)}
      ${kpi("Сумма договора", money(contract.totalAmount ?? contract.TotalAmount), "accent-gold")}
      ${kpi("Статус", "")}
    </div>
    <div class="table-wrap">
      <table>
        <thead><tr><th>Мес.</th><th>Сумма</th><th>Оплачено</th><th>Срок</th><th>Статус</th><th></th></tr></thead>
        <tbody>
          ${schedules.map((s) => {
            const id = s.id ?? s.Id;
            const isPaid = s.isPaid ?? s.IsPaid;
            const remaining = (s.expectedAmount ?? s.ExpectedAmount) - (s.paidAmount ?? s.PaidAmount);
            return `
            <tr>
              <td class="num">${s.monthNumber ?? s.MonthNumber}</td>
              <td class="num">${money(s.expectedAmount ?? s.ExpectedAmount)}</td>
              <td class="num">${money(s.paidAmount ?? s.PaidAmount)}</td>
              <td class="num">${dateStr(s.dueDate ?? s.DueDate)}</td>
              <td><span class="stamp ${isPaid ? "stamp-paid" : "stamp-unpaid"}">${isPaid ? "Оплачен" : "Ожидает"}</span></td>
              <td class="row-actions">
                ${!isPaid && canWrite ? `<button class="btn btn-primary btn-small" data-pay="${id}" data-max="${remaining}">Оплатить</button>` : ""}
              </td>
            </tr>`;
          }).join("")}
        </tbody>
      </table>
    </div>
  `);

  document.querySelectorAll("[data-pay]").forEach((b) => b.addEventListener("click", () => {
    Modal.close();
    openPaymentModal({ scheduleId: b.dataset.pay, max: Number(b.dataset.max) });
  }));
}

/* -------------------------------- Payments --------------------------------- */

async function renderPayments() {
  const canWrite = Auth.hasRole("Manager", "Cashier");
  const canManage = Auth.hasRole("Manager");
  const actions = document.getElementById("view-actions");
  actions.innerHTML = canWrite ? `<button class="btn btn-primary btn-small" id="add-payment">+ Платеж</button>` : "";
  if (canWrite) document.getElementById("add-payment").addEventListener("click", () => openPaymentPicker());

  const payments = await api("/payments");
  const root = document.getElementById("view-root");
  root.innerHTML = `
    <div class="table-wrap">
      <table>
        <thead><tr><th>Договор</th><th>Сумма</th><th>Дата</th><th>Способ</th><th>Примечание</th><th></th></tr></thead>
        <tbody>
          ${payments.length ? payments.map((p) => `
            <tr>
              <td>${esc(p.contractNumber ?? p.ContractNumber)}</td>
              <td class="num">${money(p.amount ?? p.Amount)}</td>
              <td class="num">${dateStr(p.paymentDate ?? p.PaymentDate)}</td>
              <td>${esc(p.paymentMethod ?? p.PaymentMethod)}</td>
              <td>${esc(p.notes ?? p.Notes ?? "")}</td>
              <td class="row-actions">
                ${canManage ? `<button class="btn btn-danger btn-small" data-delete="${p.id ?? p.Id}">Удалить</button>` : ""}
              </td>
            </tr>`).join("") : emptyRow(6, "Платежей пока нет.")}
        </tbody>
      </table>
    </div>
  `;

  root.querySelectorAll("[data-delete]").forEach((b) => b.addEventListener("click", () => confirmDelete(
    "Удалить платеж? Это откатит график платежей.", async () => {
      await api(`/payments/${b.dataset.delete}`, { method: "DELETE" });
      toast("Платеж удален.");
      navigateTo("payments");
    })));
}

async function openPaymentPicker() {
  const contracts = (await api("/installmentcontracts")).filter((c) => (c.status ?? c.Status) !== "Completed");
  const options = contracts.map((c) => `<option value="${c.id ?? c.Id}">${esc(c.contractNumber ?? c.ContractNumber)} — ${esc(c.customerName ?? c.CustomerName)}</option>`).join("");

  Modal.open("Выбрать договор", `
    <label>Договор
      <select id="picker-contract">${options || '<option value="">Нет активных договоров</option>'}</select>
    </label>
    <div class="modal-foot">
      <button type="button" class="btn btn-ghost" id="cancel-modal">Отмена</button>
      <button type="button" class="btn btn-primary" id="picker-next">Далее</button>
    </div>
  `);
  document.getElementById("cancel-modal").addEventListener("click", () => Modal.close());
  document.getElementById("picker-next").addEventListener("click", async () => {
    const contractId = document.getElementById("picker-contract").value;
    if (!contractId) return;
    const schedules = await api(`/paymentschedules/${contractId}`);
    const unpaid = schedules.filter((s) => !(s.isPaid ?? s.IsPaid));
    if (!unpaid.length) {
      toast("По этому договору все платежи уже оплачены.", "error");
      Modal.close();
      return;
    }
    const first = unpaid[0];
    const remaining = (first.expectedAmount ?? first.ExpectedAmount) - (first.paidAmount ?? first.PaidAmount);
    Modal.close();
    openPaymentModal({ scheduleId: first.id ?? first.Id, max: remaining, allSchedules: unpaid });
  });
}

function openPaymentModal({ scheduleId, max, allSchedules }) {
  const scheduleOptions = allSchedules ? allSchedules.map((s) => {
    const id = s.id ?? s.Id;
    const rem = (s.expectedAmount ?? s.ExpectedAmount) - (s.paidAmount ?? s.PaidAmount);
    return `<option value="${id}" data-max="${rem}" ${id === scheduleId ? "selected" : ""}>Месяц ${s.monthNumber ?? s.MonthNumber} — до ${money(rem)}</option>`;
  }).join("") : "";

  Modal.open("Принять платеж", `
    <form id="payment-form">
      ${allSchedules ? `<label>Платеж по графику
          <select name="paymentScheduleId" id="payment-schedule-select">${scheduleOptions}</select>
        </label>` : `<input type="hidden" name="paymentScheduleId" value="${scheduleId}" />`}
      <label>Сумма <span class="field-hint" id="max-amount-hint">максимум ${money(max)}</span>
        <input type="number" name="amount" min="0.01" step="0.01" max="${max}" value="${max}" required />
      </label>
      <label>Способ оплаты
        <select name="paymentMethod">
          <option value="Cash">Наличные</option>
          <option value="Card">Карта</option>
          <option value="BankTransfer">Банковский перевод</option>
          <option value="Click">Click</option>
          <option value="Payme">Payme</option>
          <option value="UzumBank">Uzum Bank</option>
        </select>
      </label>
      <label>Примечание
        <textarea name="notes"></textarea>
      </label>
      <p class="form-error" data-for="modal"></p>
      <div class="modal-foot">
        <button type="button" class="btn btn-ghost" id="cancel-modal">Отмена</button>
        <button type="submit" class="btn btn-primary">Принять платеж</button>
      </div>
    </form>
  `);

  const amountInput = document.querySelector('#payment-form input[name="amount"]');
  const scheduleSelect = document.getElementById("payment-schedule-select");
  if (scheduleSelect) {
    scheduleSelect.addEventListener("change", () => {
      const m = scheduleSelect.selectedOptions[0].dataset.max;
      amountInput.max = m;
      amountInput.value = m;
      document.getElementById("max-amount-hint").textContent = `максимум ${money(m)}`;
    });
  }

  document.getElementById("cancel-modal").addEventListener("click", () => Modal.close());
  document.getElementById("payment-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const fd = new FormData(e.target);
    const payload = {
      paymentScheduleId: fd.get("paymentScheduleId"),
      amount: Number(fd.get("amount")),
      paymentMethod: fd.get("paymentMethod"),
      notes: fd.get("notes") || null
    };
    try {
      await api("/payments", { method: "POST", body: payload });
      Modal.close();
      toast("Платеж принят.");
      navigateTo(currentView === "payments" ? "payments" : "contracts");
    } catch (err) {
      document.querySelector('.form-error[data-for="modal"]').textContent = err.message;
    }
  });
}

/* ------------------------------ Delete confirm ------------------------------ */

function confirmDelete(message, onConfirm) {
  Modal.open("Подтверждение", `
    <p>${esc(message)}</p>
    <p class="form-error" data-for="modal"></p>
    <div class="modal-foot">
      <button type="button" class="btn btn-ghost" id="cancel-modal">Отмена</button>
      <button type="button" class="btn btn-danger" id="confirm-delete">Удалить</button>
    </div>
  `);
  document.getElementById("cancel-modal").addEventListener("click", () => Modal.close());
  document.getElementById("confirm-delete").addEventListener("click", async () => {
    try {
      await onConfirm();
      Modal.close();
    } catch (err) {
      document.querySelector('.form-error[data-for="modal"]').textContent = err.message;
    }
  });
}

/* --------------------------------- Boot --------------------------------- */

(function boot() {
  if (Auth.token && !Auth.isExpired()) {
    showMainScreen();
  } else {
    Auth.logout();
    showAuthScreen();
  }
})();
