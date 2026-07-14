import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import './App.css'

type Role = 'client' | 'tradesperson' | 'admin'

const dashboards: Record<Role, { eyebrow: string; title: string; description: string; metrics: [string, string][]; actions: string[] }> = {
  client: {
    eyebrow: 'Client workspace', title: 'Your home projects, organised.', description: 'Track upcoming work, message verified pros, and keep every receipt in one place.',
    metrics: [['Next booking', 'Thu, 09:00'], ['Unread messages', '2'], ['Saved professionals', '8']], actions: ['Find a tradesperson', 'View my bookings', 'Message inbox'],
  },
  tradesperson: {
    eyebrow: 'Tradesperson workspace', title: 'Keep your business moving.', description: 'Manage leads, availability, bookings, and the trust signals clients look for.',
    metrics: [['New matched leads', '6'], ['Upcoming bookings', '4'], ['Profile completion', '92%']], actions: ['View matched jobs', 'Manage availability', 'Complete verification'],
  },
  admin: {
    eyebrow: 'Operations workspace', title: 'A clearer view of marketplace health.', description: 'Review verification, disputes, risk signals, and operational queues with complete audit trails.',
    metrics: [['Verification queue', '14'], ['Open disputes', '3'], ['Risk reviews', '7']], actions: ['Open verification queue', 'Review disputes', 'View risk cases'],
  },
}

function App() {
  const [route, setRoute] = useState(() => window.location.hash.slice(1) || '/')
  const [role, setRole] = useState<Role>('client')
  const [query, setQuery] = useState('')

  useEffect(() => {
    const updateRoute = () => setRoute(window.location.hash.slice(1) || '/')
    window.addEventListener('hashchange', updateRoute)
    return () => window.removeEventListener('hashchange', updateRoute)
  }, [])

  const navigate = (path: string) => { window.location.hash = path }
  const search = (event: FormEvent) => { event.preventDefault(); navigate(`/search?q=${encodeURIComponent(query)}`) }
  const page = route.startsWith('/dashboard') ? <Dashboard role={role} setRole={setRole} /> : route.startsWith('/search') ? <Search query={new URLSearchParams(route.split('?')[1]).get('q') ?? ''} /> : <Landing onSearch={search} query={query} setQuery={setQuery} navigate={navigate} />

  return <div className="app-shell">
    <header className="site-header">
      <a className="brand" href="#/" aria-label="Unamora home">unamora<span>.</span></a>
      <nav aria-label="Main navigation"><a href="#/search">Find a pro</a><a href="#/pricing">Pricing</a><a href="#/faq">Help centre</a></nav>
      <div className="header-actions"><a href="#/login">Sign in</a><a className="button button-small" href="#/dashboard">Get started</a></div>
    </header>
    <main>{page}</main>
    <footer><span>© 2026 Unamora</span><div><a href="#/terms">Terms</a><a href="#/privacy">Privacy</a><a href="#/contact">Contact</a></div></footer>
  </div>
}

function Landing({ onSearch, query, setQuery, navigate }: { onSearch: (event: FormEvent) => void; query: string; setQuery: (value: string) => void; navigate: (path: string) => void }) {
  return <>
    <section className="hero-section">
      <p className="eyebrow">Trusted local services, made simple</p><h1>Find the right person<br />for the job.</h1>
      <p className="hero-copy">From urgent repairs to projects you have been planning, Unamora connects you with verified professionals who are ready to help.</p>
      <form className="search-box" onSubmit={onSearch}><label htmlFor="search">What do you need help with?</label><div><input id="search" value={query} onChange={(e) => setQuery(e.target.value)} placeholder="Try “emergency plumber near me”" /><button className="button" type="submit">Search</button></div></form>
      <p className="trust-line">✓ Verified profiles &nbsp; ✓ Secure bookings &nbsp; ✓ Clear communication</p>
    </section>
    <section className="content-section"><div className="section-heading"><p className="eyebrow">One platform, every step</p><h2>Built for work that matters.</h2></div><div className="feature-grid">
      <Feature number="01" title="Find with confidence" text="Search by service, location, availability, and the proof that matters to you." />
      <Feature number="02" title="Book without guesswork" text="Keep quotes, booking details, secure payments, and updates in one clear timeline." />
      <Feature number="03" title="Grow a trusted business" text="Give skilled tradespeople the tools to showcase work and manage every opportunity." />
    </div></section>
    <section className="cta-section"><div><p className="eyebrow">Ready when you are</p><h2>Bring your next project to life.</h2></div><button className="button" onClick={() => navigate('/dashboard')}>Create your account</button></section>
  </>
}

function Dashboard({ role, setRole }: { role: Role; setRole: (role: Role) => void }) {
  const data = dashboards[role]
  return <section className="dashboard"><aside><a className="brand" href="#/">unamora<span>.</span></a><p>Workspace</p>{(['client', 'tradesperson', 'admin'] as Role[]).map((item) => <button className={role === item ? 'role active' : 'role'} onClick={() => setRole(item)} key={item}>{item}</button>)}<div className="side-links"><a href="#/search">Search</a><a href="#/settings">Settings</a></div></aside><div className="dashboard-main"><p className="eyebrow">{data.eyebrow}</p><h1>{data.title}</h1><p className="dashboard-copy">{data.description}</p><div className="metric-grid">{data.metrics.map(([label, value]) => <article className="metric" key={label}><span>{label}</span><strong>{value}</strong></article>)}</div><section className="work-panel"><div><h2>What would you like to do?</h2><p>Choose an action to get where you need to go.</p></div><div className="action-list">{data.actions.map((action) => <button key={action}>{action}<span>→</span></button>)}</div></section></div></section>
}

function Search({ query }: { query: string }) { return <section className="search-page"><p className="eyebrow">Smart search</p><h1>Professionals matched to your needs.</h1><p className="dashboard-copy">{query ? `Showing the best available matches for “${query}”.` : 'Use plain language to find a verified professional.'}</p><div className="result-grid">{['Verified and nearby', 'Available this week', 'Highly rated'].map((title, index) => <article className="result" key={title}><span className="avatar">{['AP', 'NM', 'KS'][index]}</span><div><h2>{['Apex Plumbing', 'Ndlovu Electrical', 'Kasi Build Co.'][index]}</h2><p>{title} · Johannesburg</p><small>Why this match: service, area, and availability fit your search.</small></div><button className="button button-small">View profile</button></article>)}</div></section> }
function Feature({ number, title, text }: { number: string; title: string; text: string }) { return <article className="feature"><span>{number}</span><h3>{title}</h3><p>{text}</p></article> }
export default App
