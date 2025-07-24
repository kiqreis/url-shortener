"use client"

import { FormEvent, useEffect, useState } from "react"

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

const LinkCuteIcon = () => (
  <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M6 5v14m0-14h4m-4 14h5" />
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M14 5v7h3m0 0l-3 7m3-7l3-3" />
  </svg>
)

const SunIcon = () => (
  <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" />
  </svg>
)

const MoonIcon = () => (
  <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
  </svg>
)

const UserCircleIcon = () => (
  <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5.121 17.804A13.937 13.937 0 0112 16c2.5 0 4.847.655 6.879 1.804M15 10a3 3 0 11-6 0 3 3 0 016 0zm6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
  </svg>)

const BASE_URL = "http://localhost:5181";

interface ShortenUrlRequest {
  originalUrl: string;
  customCode?: string;
}

interface ShortenUrlResponse {
  originalUrl: string;
  shortUrl: string;
  shortCode: string;
  createdAt: string;
  remainingShortenings: number;
}

interface ShortenedUrl {
  originalUrl: string;
  shortUrl: string;
  shortCode: string;
  remainingShortenings: number;
  createdAt: string;
}


interface LoginModalProps {
  isOpen: boolean;
  darkMode: boolean;
  onClose: () => void;
}

class ApiService {

  private static async handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
      const error = await response.text();

      throw new Error(`Error: ${response.status} - ${error}`);
    }

    return response.json();
  }

  static async shortenUrl(request: ShortenUrlRequest): Promise<ShortenUrlResponse> {
    const response = await fetch(`${BASE_URL}/v1/urls/shorten`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(request)
    });

    return this.handleResponse<ShortenUrlResponse>(response);
  }

  static async getAllUrls(): Promise<ShortenedUrl[]> {
    const response = await fetch(`${BASE_URL}/v1/urls`);

    return this.handleResponse<ShortenedUrl[]>(response);
  }

}

const LoginModal = ({ isOpen, darkMode, onClose }: LoginModalProps) => {
  const [isLogin, setIsLogin] = useState(true);

  if (!open) return null;

  const handleModalContentClick = (e: FormEvent) => {
    e.stopPropagation();
  }

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-60 backdrop-blur-sm"
      onClick={onClose}
    >
      <div
        onClick={handleModalContentClick}
        className={`rounded-lg shadow-2xl border-0 transition-colors duration-200 w-full max-w-md mx-4 ${darkMode ? 'bg-gray-800 text-white' : 'bg-white text-gray-800'
          }`}
      >
        <div className="p-6">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-2xl font-bold">{isLogin ? 'Login' : 'Cadastro'}</h2>
            <button onClick={onClose} className={`p-1 rounded-full transition-colors ${darkMode ? 'hover:bg-gray-700' : 'hover:bg-gray-200'}`}>&times;</button>
          </div>

          <div className="flex border-b mb-6">
            <button
              onClick={() => setIsLogin(true)}
              className={`py-2 px-4 font-semibold w-1/2 ${isLogin ? (darkMode ? 'border-b-2 border-blue-500 text-blue-500' : 'border-b-2 border-red-400 text-red-400') : (darkMode ? 'text-gray-400' : 'text-gray-500')}`}
            >
              Entrar
            </button>
            <button
              onClick={() => setIsLogin(false)}
              className={`py-2 px-4 font-semibold w-1/2 ${!isLogin ? (darkMode ? 'border-b-2 border-blue-500 text-blue-500' : 'border-b-2 border-red-400 text-red-400') : (darkMode ? 'text-gray-400' : 'text-gray-500')}`}
            >
              Criar Conta
            </button>
          </div>

          <form>
            {!isLogin && (
              <div className="mb-4">
                <label className="block text-sm font-medium mb-1">Nome</label>
                <input type="text" placeholder="Seu nome completo" className={`w-full p-3 border rounded-lg focus:outline-none focus:ring-2 ${darkMode ? 'bg-gray-700 border-gray-600 focus:ring-blue-500' : 'bg-gray-50 border-gray-300 focus:ring-red-400'}`} />
              </div>
            )}

            <div className="mb-4">
              <label className="block text-sm font-medium mb-1">Email</label>
              <input type="email" placeholder="voce@exemplo.com" className={`w-full p-3 border rounded-lg focus:outline-none focus:ring-2 ${darkMode ? 'bg-gray-700 border-gray-600 focus:ring-blue-500' : 'bg-gray-50 border-gray-300 focus:ring-red-400'}`} />
            </div>

            <div className="mb-6">
              <label className="block text-sm font-medium mb-1">Senha</label>
              <input type="password" placeholder="••••••••" className={`w-full p-3 border rounded-lg focus:outline-none focus:ring-2 ${darkMode ? 'bg-gray-700 border-gray-600 focus:ring-blue-500' : 'bg-gray-50 border-gray-300 focus:ring-red-400'}`} />
            </div>

            <button type="submit" className={`w-full py-3 font-semibold rounded-lg text-white transition-colors ${darkMode ? 'bg-blue-500 hover:bg-blue-600' : 'bg-red-400 hover:bg-red-500'}`}>
              {isLogin ? 'Entrar' : 'Criar Conta'}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default function URLShortener() {

  const [url, setUrl] = useState("");
  const [shortenedUrls, setShortenedUrls] = useState<ShortenedUrl[]>([]);
  const [copiedId, setCopiedId] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isLoadingUrls, setIsLoadingUrls] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [darkMode, setDarkMode] = useState(true);

  useEffect(() => {
    loadingExistingUrls();
  }, []);

  const loadingExistingUrls = async () => {
    try {
      setIsLoadingUrls(true);

      const urls = await ApiService.getAllUrls();

      setShortenedUrls(urls);
    } catch (e) {
      setError("Error loading existing URLs");
    } finally {
      setIsLoadingUrls(false);
    }
  }

  const handleShorten = async () => {
    if (!url.trim()) return

    setLoading(true);
    setError(null);

    try {
      const response = await ApiService.shortenUrl({ originalUrl: url });

      const newUrl: ShortenedUrl = {
        originalUrl: response.originalUrl,
        shortUrl: response.shortUrl,
        shortCode: response.shortCode,
        remainingShortenings: response.remainingShortenings,
        createdAt: response.createdAt
      };

      setShortenedUrls(previous => [newUrl, ...previous]);
      setUrl("");
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Error shortening URL');
    } finally {
      setLoading(false);
    }
  }

  const copyToClipboard = async (text: string, id: string) => {
    try {
      await navigator.clipboard.writeText(text);

      setCopiedId(id);
      setTimeout(() => setCopiedId(null), 2000);
    } catch (e) {
      setError("Error copying URL");
    }
  }

  const toggleDarkMode = () => {
    setDarkMode(!darkMode);
  }

  return (
    <div className=
      {`min-h-screen transition-colors duration-200 ${darkMode ? 'dark bg-gray-900' : 'bg-slate-50'}`}
    >
      <header className=
        {`border-b backdrop-blur-sm transition-colors duration-200 ${darkMode
          ? 'border-gray-700 bg-gray-800/50'
          : 'border-gray-100 bg-white/50'
          }`
        }
      >
        <div className="container mx-auto px-4 py-6">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div
                className={`p-2 rounded-lg transition-colors
                ${darkMode
                    ? 'bg-blue-100 text-blue-600'
                    : 'bg-red-100 text-red-400'
                  }`}
              >
                <LinkCuteIcon />
              </div>
              <div>
                <h1 className={`text-2xl font-bold transition-colors duration-200 ${darkMode ? 'text-white' : 'text-gray-800'
                  }`}>
                  LinkCute
                </h1>
                <p className={`text-sm transition-colors duration-200 ${darkMode ? 'text-gray-200' : 'text-gray-600'
                  }`}>
                  Simple and fast URLs button
                </p>
              </div>
            </div>

            <div className="flex items-center gap-3">
              <button
                onClick={() => setIsModalOpen(true)}
                title="Login / Cadastro"
                className={`p-2 rounded-lg transition-colors duration-200 ${darkMode ? 'bg-gray-700 hover:bg-gray-600 text-gray-200' : 'bg-gray-100 hover:bg-gray-200 text-gray-600'}`}
              >
                <UserCircleIcon />
              </button>

              <button
                onClick={toggleDarkMode}
                className={`p-2 rounded-lg transition-colors duration-200 ${darkMode
                  ? 'bg-gray-700 hover:bg-gray-600 text-yellow-400'
                  : 'bg-gray-100 hover:bg-gray-200 text-gray-600'
                  }`}
              >
                {darkMode ? <SunIcon /> : <MoonIcon />}
              </button>
            </div>
          </div>
        </div>
      </header>

      <main className="container mx-auto px-4 py-12">
        <div className="text-center mb-12">
          <h2 className={`text-4xl font-bold mb-4 transition-colors duration-200 ${darkMode ? 'text-white' : 'text-gray-800'
            }`}>
            Short your URLs in seconds
          </h2>
          <p className={`text-lg max-w-2xl mx-auto transition-colors duration-200 ${darkMode ? 'text-gray-200' : 'text-gray-600'
            }`}>
            Turn long links into short, easy-to-share URLs. Track clicks and manage your links easily.
          </p>
        </div>

        <div className={`max-w-2xl mx-auto mb-12 backdrop-blur-sm rounded-lg shadow-lg border-0 transition-colors duration-200 ${darkMode ? 'bg-gray-800/80' : 'bg-white/80'
          }`}>
          <div className={`p-6 text-center border-b transition-colors duration-200 ${darkMode ? 'border-gray-700' : 'border-gray-100'
            }`}>
            <h3 className={`text-2xl font-semibold mb-2 transition-colors duration-200 ${darkMode ? 'text-white' : 'text-gray-800'
              }`}>
              Shorten URL
            </h3>
            <p className={`transition-colors duration-200 ${darkMode ? 'text-gray-200' : 'text-gray-600'
              }`}>
              Paste your long URL below and get a short link instantly
            </p>
          </div>
          <div className="p-6">
            <div className="flex gap-2">
              <div className="relative flex-1">
                <div
                  className={`absolute left-3 top-1/2 transform -translate-y-1/2 transition-colors duration-200 
                  ${darkMode ? 'text-gray-200' : 'text-gray-400'
                    }`
                  }
                >
                  <LinkIcon />
                </div>
                <input
                  type="url"
                  placeholder="https://example.com/your-url-is-too-long"
                  value={url}
                  onChange={(e) => setUrl(e.target.value)}
                  className={`w-full pl-10 pr-4 py-3 border rounded-lg focus:outline-none focus:ring-2 focus:border-transparent transition-colors duration-200 ${darkMode
                    ? 'border-gray-600 bg-gray-700 text-white placeholder-gray-400 focus:ring-blue-500'
                    : 'border-gray-200 bg-white text-gray-800 placeholder-gray-500 focus:ring-red-400'
                    }`}
                />
              </div>
              <button
                onClick={handleShorten}
                disabled={!url || loading}
                className={`px-8 py-3 font-medium rounded-lg transition-colors duration-200
                ${!url || loading
                    ? `${darkMode
                      ? 'bg-gray-700 text-gray-200'
                      : 'bg-gray-200 text-gray-500'
                    } cursor-not-allowed`
                    : `${darkMode ? 'bg-blue-500 hover:bg-blue-600 text-white' : 'bg-red-400 hover:bg-red-500 text-white'}`
                  }`}
              >
                {loading ? 'Shortening...' : 'Shorten'}
              </button>
            </div>
          </div>
        </div>

        {shortenedUrls.length > 0 && (
          <div className="max-w-4xl mx-auto">
            <h3 className={`text-2xl font-bold mb-6 ${darkMode ? 'text-gray-200' : 'text-gray-800'}`}>Your Shortened URLs</h3>
            <div className="space-y-4">
              {shortenedUrls.map((item) => (
                <div
                  key={item.shortCode}
                  className={`backdrop-blur-sm rounded-lg shadow-md border-0 transition-colors ${darkMode ? 'bg-gray-800/70' : 'bg-white/80'
                    }`}
                >
                  <div className="p-6">
                    <div className="flex items-center justify-between gap-4">
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 mb-2">
                          <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                          <span className="text-sm font-medium text-green-600">Active</span>
                          <span className={`text-sm ${darkMode ? 'text-gray-200' : 'text-gray-500'}`}>• {item.remainingShortenings} remaining shortenings</span>
                        </div>
                        <p className={`text-lg font-semibold mb-1 ${darkMode ? 'text-gray-200' : 'text-gray-800'}`}>{item.shortUrl}</p>
                        <p className={`text-sm truncate ${darkMode ? 'text-gray-400' : 'text-gray-600'}`}>{item.originalUrl}</p>
                      </div>
                      <div className="flex gap-2">
                        <button
                          onClick={() => copyToClipboard(item.shortUrl, item.shortCode)}
                          className="p-2 border border-green-500 text-green-600 hover:bg-green-500 hover:text-white rounded-lg transition-colors"
                        >
                          {copiedId === item.shortCode ? <CheckIcon /> : <CopyIcon />}
                        </button>
                        <button
                          onClick={() => window.open(item.originalUrl, "_blank")}
                          className={`p-2 border rounded-lg transition-colors ${darkMode
                            ? 'border-gray-400 text-gray-200 hover:bg-gray-200 hover:text-gray-900'
                            : 'border-gray-400 text-gray-600 hover:bg-gray-600 hover:text-white'
                            }`}
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
          <h3 className={`text-2xl font-bold mb-12 text-center ${darkMode ? 'text-white' : 'text-gray-800'}`}>Why use LinkCute?</h3>
          <div className="grid md:grid-cols-3 gap-8">
            <div className="text-center">
              <div className="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center mx-auto mb-4 text-red-400">
                <ScissorsIcon />
              </div>
              <h4 className={`font-semibold mb-2 ${darkMode ? 'text-white' : 'text-gray-800'}`}>Fast and Simple</h4>
              <p className={`text-sm ${darkMode ? 'text-gray-300' : 'text-gray-600'}`}>Shorten URLs in seconds with our intuitive interface</p>
            </div>
            <div className="text-center">
              <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center mx-auto mb-4 text-green-600">
                <LinkIcon />
              </div>
              <h4 className={`font-semibold mb-2 ${darkMode ? 'text-white' : 'text-gray-800'}`}>Trusted Links</h4>
              <p className={`text-sm ${darkMode ? 'text-gray-300' : 'text-gray-600'}`}>Secure and stable URLs to share with confidence</p>
            </div>
            <div className="text-center">
              <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mx-auto mb-4 text-blue-600">
                <ExternalLinkIcon />
              </div>
              <h4 className={`font-semibold mb-2 ${darkMode ? 'text-white' : 'text-gray-800'}`}>Track Clicks</h4>
              <p className={`text-sm ${darkMode ? 'text-gray-300' : 'text-gray-600'}`}>Monitor the performance of your links in real time</p>
            </div>
          </div>
        </div>
      </main>

      <footer
        className={`border-t backdrop-blur-sm transition-colors duration-200 ${darkMode
          ? 'border-gray-700 bg-gray-800/50'
          : 'border-gray-100 bg-white/50'
          }`
        }
      >
        <div className="container mx-auto px-4 py-8">
          <div className="text-center">
            <p className={`text-sm ${darkMode ? 'text-gray-300' : 'text-gray-600'}`}>© 2025 LinkCute. All rights reserved.</p>
            <p className={`text-xs mt-2 ${darkMode ? 'text-gray-400' : 'text-gray-500'}`}>Simple, fast and reliable URL shortener</p>
          </div>
        </div>
      </footer>
    </div>
  )
}