// Shared helpers for the hoaii.vn functional tests.
const BASE = process.env.BASE || 'http://localhost:5167';

// ---------- tiny test framework ----------
const results = [];
let current = null;

async function test(name, fn) {
  current = { name, checks: [], error: null };
  try {
    await fn();
  } catch (e) {
    current.error = e.message;
  }
  results.push(current);
  const failed = current.error || current.checks.some(c => !c.ok);
  const icon = failed ? 'FAIL' : ' OK ';
  console.log(`[${icon}] ${name}`);
  for (const c of current.checks.filter(c => !c.ok)) console.log(`        ✗ ${c.msg}`);
  if (current.error) console.log(`        ✗ threw: ${current.error}`);
  current = null;
}

function check(ok, msg) {
  if (current) current.checks.push({ ok: !!ok, msg });
  else if (!ok) console.log('  ✗ (outside test) ' + msg);
  return !!ok;
}
const eq = (a, b, msg) => check(a === b, `${msg} — mong "${b}", nhận "${a}"`);
const has = (hay, needle, msg) => check(String(hay).includes(needle), `${msg} — không thấy "${needle}"`);
const notHas = (hay, needle, msg) => check(!String(hay).includes(needle), `${msg} — không nên có "${needle}"`);

function summary(title) {
  const failed = results.filter(r => r.error || r.checks.some(c => !c.ok));
  console.log(`\n${'='.repeat(60)}\n${title}: ${results.length - failed.length}/${results.length} PASS`);
  if (failed.length) {
    console.log(`\n${failed.length} LỖI:`);
    for (const f of failed) {
      console.log(`  • ${f.name}`);
      for (const c of f.checks.filter(c => !c.ok)) console.log(`      ${c.msg}`);
      if (f.error) console.log(`      threw: ${f.error}`);
    }
  }
  return failed.length;
}

// ---------- http session with a real cookie jar ----------
function session() {
  const jar = new Map();
  const cookie = () => [...jar].map(([k, v]) => `${k}=${v}`).join('; ');
  const absorb = r => {
    for (const c of r.headers.getSetCookie?.() || []) {
      const [pair] = c.split(';');
      const i = pair.indexOf('=');
      const name = pair.slice(0, i), val = pair.slice(i + 1);
      if (val === '' ) jar.delete(name); else jar.set(name, val);
    }
  };

  const api = {
    jar,
    async get(path) {
      const r = await fetch(BASE + path, { headers: { cookie: cookie() }, redirect: 'manual' });
      absorb(r);
      const body = await r.text();
      return { status: r.status, body, location: r.headers.get('location'), headers: r.headers };
    },
    async post(path, data, opts = {}) {
      const body = data instanceof FormData ? data : new URLSearchParams(data).toString();
      const headers = { cookie: cookie(), ...(opts.headers || {}) };
      if (!(data instanceof FormData)) headers['content-type'] = 'application/x-www-form-urlencoded';
      const r = await fetch(BASE + path, { method: 'POST', body, headers, redirect: 'manual' });
      absorb(r);
      return { status: r.status, body: await r.text(), location: r.headers.get('location'), headers: r.headers };
    },
    /// GET a page and pull its antiforgery token.
    async token(path) {
      const { body } = await api.get(path);
      return (body.match(/name="__RequestVerificationToken"[^>]*value="([^"]+)"/) || [])[1];
    },
    async adminLogin(email = 'admin@hoaii.vn', password = 'Hoaii@2026') {
      const t = await api.token('/admin/dang-nhap');
      const r = await api.post('/admin/dang-nhap', { Email: email, Password: password, __RequestVerificationToken: t });
      return r;
    },
  };
  return api;
}

module.exports = { BASE, test, check, eq, has, notHas, summary, session, results };
