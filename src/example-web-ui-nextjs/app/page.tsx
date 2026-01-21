export default function Home() {
  return (
    <div className="flex min-h-screen flex-col bg-gradient-to-br from-purple-900 via-purple-950 to-amber-950 dark:from-purple-950 dark:via-purple-900 dark:to-amber-900">
      <main className="flex flex-1 items-center justify-center px-8 py-16 sm:px-16">
        <div className="w-full max-w-4xl">
          <div className="flex flex-col items-center gap-8 text-center">
            <div className="mb-4">
              <h1 className="text-5xl font-bold leading-tight tracking-tight text-amber-400 dark:text-amber-300 sm:text-6xl md:text-7xl">
                Project
              </h1>
              <div className="mt-2 h-1 w-24 bg-gradient-to-r from-purple-400 to-amber-400 mx-auto"></div>
            </div>
            
            <p className="max-w-2xl text-xl leading-relaxed text-purple-100 dark:text-purple-200 sm:text-2xl">
              Your project content goes here. Build something amazing with our amber and deep purple theme.
            </p>

            <div className="mt-16 grid w-full grid-cols-1 gap-6 sm:grid-cols-3">
              <div className="rounded-xl border border-purple-400/20 bg-purple-900/30 p-6 backdrop-blur-sm dark:bg-purple-800/20">
                <div className="mb-3 text-2xl font-bold text-amber-400">Feature One</div>
                <p className="text-purple-200">
                  Describe your first key feature or highlight here.
                </p>
              </div>
              <div className="rounded-xl border border-purple-400/20 bg-purple-900/30 p-6 backdrop-blur-sm dark:bg-purple-800/20">
                <div className="mb-3 text-2xl font-bold text-amber-400">Feature Two</div>
                <p className="text-purple-200">
                  Describe your second key feature or highlight here.
                </p>
              </div>
              <div className="rounded-xl border border-purple-400/20 bg-purple-900/30 p-6 backdrop-blur-sm dark:bg-purple-800/20">
                <div className="mb-3 text-2xl font-bold text-amber-400">Feature Three</div>
                <p className="text-purple-200">
                  Describe your third key feature or highlight here.
                </p>
              </div>
            </div>
          </div>
        </div>
      </main>

      <footer className="border-t border-purple-400/20 bg-purple-900/20 backdrop-blur-sm">
        <div className="mx-auto max-w-4xl px-8 py-8 sm:px-16">
          <div className="flex flex-col items-center gap-4 sm:flex-row sm:justify-between">
            <div className="text-purple-200">
              <p className="text-sm">© {new Date().getFullYear()} Project Name</p>
            </div>
            <nav className="flex flex-wrap items-center justify-center gap-6">
              <a
                href="#"
                className="text-sm text-purple-200 transition-colors hover:text-amber-400 focus:outline-none focus:ring-2 focus:ring-amber-400 focus:ring-offset-2 focus:ring-offset-purple-900 rounded"
              >
                Link One
              </a>
              <a
                href="#"
                className="text-sm text-purple-200 transition-colors hover:text-amber-400 focus:outline-none focus:ring-2 focus:ring-amber-400 focus:ring-offset-2 focus:ring-offset-purple-900 rounded"
              >
                Link Two
              </a>
              <a
                href="#"
                className="text-sm text-purple-200 transition-colors hover:text-amber-400 focus:outline-none focus:ring-2 focus:ring-amber-400 focus:ring-offset-2 focus:ring-offset-purple-900 rounded"
              >
                Link Three
              </a>
            </nav>
          </div>
        </div>
      </footer>
    </div>
  );
}
