"use client"

import { useState } from "react"

const ScissorsIcon = () => (
  <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M14.828 14.828a4 4 0 01-5.656 0M9 10h1m4 0h1m-6 8a2 2 0 002 2h8a2 2 0 002-2M6 10a2 2 0 002-2h8a2 2 0 002 2M6 10a2 2 0 012-2h8a2 2 0 012 2" />
  </svg>
)

const LinkIcon = () => (
  <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13.828 10.172a4 4 0 00-5.656 0l-4 4a4 4 0 105.656 5.656l1.102-1.101m-.758-4.899a4 4 0 005.656 0l4-4a4 4 0 00-5.656-5.656l-1.1 1.1" />
  </svg>
)

const CopyIcon = () => (
  <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
  </svg>
)

const CheckIcon = () => (
  <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
  </svg>
)

const ExternalLinkIcon = () => (
  <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
  </svg>
)

const LinkCutIcon1 = () => (
  <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M6 5v14m0-14h4m-4 14h5" />
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M14 5v7h3m0 0l-3 7m3-7l3-3" />
  </svg>
)

export default function URLShortener() {
  const [url, setUrl] = useState("")
  const [shortenedUrls, setShortenedUrls] = useState<
    Array<{ id: string; original: string; shortened: string; clicks: number }>
  >([])
  const [copiedId, setCopiedId] = useState<string | null>(null)

  const handleShorten = () => {
    if (!url) return

    const id = Math.random().toString(36).substring(2, 8)

    const newUrl = {
      id,
      original: url,
      shortened: `https://link.cute/${id}`,
      clicks: Math.floor(Math.random() * 100),
    }

    setShortenedUrls((prev) => [newUrl, ...prev])
    setUrl("")
  }

  const copyToClipboard = async (text: string, id: string) => {
    await navigator.clipboard.writeText(text)
    setCopiedId(id)
    setTimeout(() => setCopiedId(null), 2000)
  }

  return (
    <div className="min-h-screen bg-slate-50">
      <header className="border-b border-gray-100 bg-white/50 backdrop-blur-sm">
        <div className="container mx-auto px-4 py-6">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-red-100 rounded-lg text-red-400">
              <LinkCutIcon1 />
            </div>
            <div>
              <h1 className="text-2xl font-bold text-gray-800">LinkCute</h1>
              <p className="text-sm text-gray-600">Simple and fast URLs button</p>
            </div>
          </div>
        </div>
      </header>

      <main className="container mx-auto px-4 py-12">
        <div className="text-center mb-12">
          <h2 className="text-4xl font-bold text-gray-800 mb-4">Short your URLs in seconds</h2>
          <p className="text-lg text-gray-600 max-w-2xl mx-auto">
            Turn long links into short, easy-to-share URLs. Track clicks and manage your links easily.
          </p>
        </div>

        <div className="max-w-2xl mx-auto mb-12 bg-white/80 backdrop-blur-sm rounded-lg shadow-lg border-0">
          <div className="p-6 text-center border-b border-gray-100">
            <h3 className="text-2xl font-semibold text-gray-800 mb-2">Shorten URL</h3>
            <p className="text-gray-600">Paste your long URL below and get a short link instantly</p>
          </div>
          <div className="p-6">
            <div className="flex gap-2">
              <div className="relative flex-1">
                <div className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400">
                  <LinkIcon />
                </div>
                <input
                  type="url"
                  placeholder="https://example.com/your-url-is-too-long"
                  value={url}
                  onChange={(e) => setUrl(e.target.value)}
                  className="w-full pl-10 pr-4 py-3 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-400 focus:border-transparent text-gray-800 placeholder-gray-500"
                />
              </div>
              <button
                onClick={handleShorten}
                disabled={!url}
                className="px-8 py-3 bg-red-400 hover:bg-red-500 disabled:bg-gray-300 disabled:cursor-not-allowed disabled:text-gray-600 text-white font-medium rounded-lg transition-colors"
              >
                Shorten
              </button>
            </div>
          </div>
        </div>

        {shortenedUrls.length > 0 && (
          <div className="max-w-4xl mx-auto">
            <h3 className="text-2xl font-bold text-gray-800 mb-6">Your Shortened URLs</h3>
            <div className="space-y-4">
              {shortenedUrls.map((item) => (
                <div key={item.id} className="bg-white/80 backdrop-blur-sm rounded-lg shadow-md border-0">
                  <div className="p-6">
                    <div className="flex items-center justify-between gap-4">
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 mb-2">
                          <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                          <span className="text-sm font-medium text-green-600">Active</span>
                          <span className="text-sm text-gray-500">• {item.clicks} clicks</span>
                        </div>
                        <p className="text-lg font-semibold text-gray-800 mb-1">{item.shortened}</p>
                        <p className="text-sm text-gray-600 truncate">{item.original}</p>
                      </div>
                      <div className="flex gap-2">
                        <button
                          onClick={() => copyToClipboard(item.shortened, item.id)}
                          className="p-2 border border-green-500 text-green-600 hover:bg-green-500 hover:text-white rounded-lg transition-colors"
                        >
                          {copiedId === item.id ? <CheckIcon /> : <CopyIcon />}
                        </button>
                        <button
                          onClick={() => window.open(item.original, "_blank")}
                          className="p-2 border border-gray-300 text-gray-600 hover:bg-gray-50 rounded-lg transition-colors"
                        >
                          <ExternalLinkIcon />
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        <div className="max-w-4xl mx-auto mt-20">
          <h3 className="text-2xl font-bold text-gray-800 text-center mb-12">Why use LinkCute?</h3>
          <div className="grid md:grid-cols-3 gap-8">
            <div className="text-center">
              <div className="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center mx-auto mb-4 text-red-400">
                <ScissorsIcon />
              </div>
              <h4 className="font-semibold text-gray-800 mb-2">Fast and Simple</h4>
              <p className="text-gray-600 text-sm">Shorten URLs in seconds with our intuitive interface</p>
            </div>
            <div className="text-center">
              <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center mx-auto mb-4 text-green-600">
                <LinkIcon />
              </div>
              <h4 className="font-semibold text-gray-800 mb-2">Trusted Links</h4>
              <p className="text-gray-600 text-sm">Secure and stable URLs to share with confidence</p>
            </div>
            <div className="text-center">
              <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mx-auto mb-4 text-blue-600">
                <ExternalLinkIcon />
              </div>
              <h4 className="font-semibold text-gray-800 mb-2">Track Clicks</h4>
              <p className="text-gray-600 text-sm">Monitor the performance of your links in real time</p>
            </div>
          </div>
        </div>
      </main>

      <footer className="border-t border-gray-100 bg-white/50 backdrop-blur-sm mt-20">
        <div className="container mx-auto px-4 py-8">
          <div className="text-center">
            <p className="text-gray-600 text-sm">© 2025 LinkCute. All rights reserved.</p>
            <p className="text-gray-500 text-xs mt-2">Simple, fast and reliable URL shortener</p>
          </div>
        </div>
      </footer>
    </div>
  )
}