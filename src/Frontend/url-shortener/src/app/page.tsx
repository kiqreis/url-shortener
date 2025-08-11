"use client"

import { ChangeEventHandler, FormEvent, useEffect, useState } from "react"

import {
  Copy,
  Check,
  ExternalLink,
  Sun,
  Moon,
  User,
  ArrowRight
} from "lucide-react"

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
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    password: ""
  })

  if (!isOpen) return null;

  const handleModalContentClick = (e: FormEvent) => {
    e.stopPropagation();
  }

  const handleInputChange: ChangeEventHandler<HTMLInputElement> = (e) => {
    const { name, value } = e.currentTarget;

    setFormData(prev => ({
      ...prev, [name]: value
    }));

    if (error) setError("");
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError("");

    try {
      const endpoint = isLogin ? "v1/users/login" : "v1/users/register";
      const payload = isLogin
        ? { email: formData.email, password: formData.password }
        : { name: formData.name, email: formData.email, password: formData.password };

      const response = await fetch(`${BASE_URL}/${endpoint}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify(payload)
      })

      if (response.ok) {
        setFormData({ name: "", email: "", password: "" });
        onClose();
        window.location.reload();
      } else {
        const errorData = await response.json();
        setError(errorData.message || "Error when making login/registration");
      }
    } catch (error) {
      setError("Connection error. Check your internet.")
    } finally {
      setIsLoading(false);
    }
  }

  const resetForm = () => {
    setFormData({ name: "", email: "", password: "" });
    setError("");
  }

  const handleTabChange = (loginMode: boolean) => {
    setIsLogin(loginMode);
    resetForm();
  }

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-zinc-900 bg-opacity-50"
      onClick={onClose}
    >
      <div
        onClick={handleModalContentClick}
        className={`w-full max-w-sm mx-4 ${darkMode ? 'bg-zinc-900 text-white' : 'bg-white text-zinc-900'
          } border-2 ${darkMode ? 'border-blue-500' : 'border-red-400'
          }`}
        style={{ borderRadius: '12px 2px 12px 2px' }}
      >
        <div className="p-8">
          <div className="mb-8">
            <h2 className={`text-3xl font-black mb-2 ${darkMode ? 'text-blue-400' : 'text-red-400'
              }`}>
              {isLogin ? "Enter" : "Create account"}
            </h2>
            <p className={`text-sm ${darkMode ? 'text-zinc-300' : 'text-zinc-600'
              }`}>
              {isLogin ? "Access your account" : "Join LinkCute"}
            </p>
          </div>

          <div className="flex mb-8">
            <button
              onClick={() => handleTabChange(true)}
              className={`text-sm font-bold mr-6 pb-1 border-b-2 transition-colors ${isLogin
                ? (darkMode ? 'border-blue-400 text-blue-400' : 'border-red-400 text-red-400')
                : 'border-transparent text-zinc-400'
                }`}
            >
              Enter
            </button>
            <button
              onClick={() => handleTabChange(false)}
              className={`text-sm font-bold pb-1 border-b-2 transition-colors ${!isLogin
                ? (darkMode ? 'border-blue-400 text-blue-400' : 'border-red-400 text-red-400')
                : 'border-transparent text-zinc-400'
                }`}
            >
              Register
            </button>
          </div>

          <div onSubmit={handleSubmit}>
            {!isLogin && (
              <div className="mb-6">
                <input
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  placeholder="Your name"
                  className={`w-full p-4 border-2 bg-transparent font-medium placeholder-zinc-500 focus:outline-none transition-colors ${darkMode
                    ? 'border-zinc-600 focus:border-blue-400 text-white'
                    : 'border-zinc-300 focus:border-red-400 text-zinc-900'
                    }`}
                  style={{ borderRadius: '8px 2px 8px 2px' }}
                />
              </div>
            )}

            <div className="mb-6">
              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleInputChange}
                placeholder="email@example.com"
                className={`w-full p-4 border-2 bg-transparent font-medium placeholder-zinc-500 focus:outline-none transition-colors ${darkMode
                  ? 'border-zinc-600 focus:border-blue-400 text-white'
                  : 'border-zinc-300 focus:border-red-400 text-zinc-900'
                  }`}
                style={{ borderRadius: '2px 8px 2px 8px' }}
              />
            </div>

            <div className="mb-8">
              <input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleInputChange}
                placeholder="••••••••"
                className={`w-full p-4 border-2 bg-transparent font-medium placeholder-gray-500 focus:outline-none transition-colors ${darkMode
                  ? 'border-zinc-600 focus:border-blue-400 text-white'
                  : 'border-zinc-300 focus:border-red-400 text-zinc-900'
                  }`}
                style={{ borderRadius: '8px 2px 8px 2px' }}
              />
            </div>

            {error && (
              <div className="mb-6 p-4 bg-red-100 border-l-4 border-red-500 text-red-700 text-sm">
                {error}
              </div>
            )}

            <button
              type="submit"
              disabled={isLoading}
              onClick={handleSubmit}
              className={`w-full py-4 font-black text-white transition-all transform hover:scale-105 ${darkMode
                ? 'bg-blue-500 hover:bg-blue-600'
                : 'bg-red-400 hover:bg-red-500'
                }`}
              style={{ borderRadius: '2px 8px 2px 8px' }}
            >
              {isLoading ? 'Loading...' : (isLogin ? 'Enter →' : 'Create account →')}
            </button>
          </div>
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
      console.log(`Could not load existing URLs ${e}`)
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
    <div className={`min-h-screen flex flex-col transition-colors ${darkMode ? 'bg-zinc-900' : 'bg-zinc-50'
      }`}>
      <nav className={`px-6 py-4 border-b-1 ${darkMode ? 'border-zinc-700' : 'border-zinc-200'
        }`}>
        <div className="max-w-6xl mx-auto flex items-center justify-between">
          <div className="flex items-center">
            <div className={`w-8 h-8 flex items-center justify-center font-black text-xl ${darkMode ? 'text-blue-400' : 'text-red-400'
              }`}>
              Z
            </div>
            <div className="-ml-2">
              <h1 className={`text-xl font-black ${darkMode ? 'text-white' : 'text-zinc-900'
                }`}>
                ipply
              </h1>
            </div>
          </div>

          <div className="flex items-center gap-3">
            <button
              onClick={() => setIsModalOpen(true)}
              className={`p-2 transition-colors ${darkMode
                ? 'text-gray-300 hover:text-white'
                : 'text-zinc-600 hover:text-zinc-900'
                }`}
            >
              <User size={18} />
            </button>

            <button
              onClick={toggleDarkMode}
              className={`p-2 transition-colors ${darkMode
                ? 'text-yellow-400 hover:text-yellow-300'
                : 'text-zinc-600 hover:text-zinc-900'
                }`}
            >
              {darkMode ? <Sun size={18} /> : <Moon size={18} />}
            </button>
          </div>
        </div>
      </nav>

      <main className="flex-1 max-w-4xl mx-auto px-6 w-full">
        <div className="pt-16 pb-12">
          <div className="max-w-2xl">
            <h2 className={`text-4xl font-medium mb-6 leading-tight ${darkMode ? 'text-white' : 'text-zinc-900'
              }`}>
              Short your URLs&nbsp;
              <span className={`${darkMode ? 'text-blue-400' : 'text-red-400'
                }`}>
                in seconds
              </span>
            </h2>
          </div>

          <div className="mb-4">
            <div className="flex gap-3">
              <input
                type="url"
                placeholder="https://example.com/your-url-is-too-long"
                value={url}
                onChange={(e) => setUrl(e.target.value)}
                className={`flex-1 p-4 text-lg border-2 bg-transparent font-medium placeholder-gray-500 focus:outline-none transition-all ${darkMode
                  ? 'border-zinc-600 focus:border-blue-400 text-white'
                  : 'border-zinc-300 focus:border-red-400 text-zinc-900'
                  }`}
                style={{ borderRadius: '8px 2px 8px 2px' }}
              />
              <button
                onClick={handleShorten}
                disabled={!url || loading}
                className={`px-8 font-black text-white transition-all transform hover:scale-105 disabled:opacity-50 disabled:transform-none ${darkMode ? 'bg-blue-500 hover:bg-blue-600' : 'bg-red-400 hover:bg-red-500'
                  }`}
                style={{ borderRadius: '2px 8px 2px 8px' }}
              >
                {loading ? '...' : <ArrowRight size={20} />}
              </button>
            </div>
            {error && (
              <div className="mt-3 p-3 bg-red-100 border-l-4 border-red-500 text-red-700 text-sm">
                {error}
              </div>
            )}
          </div>
        </div>

        {shortenedUrls.length > 0 && (
          <div className="mb-8">
            <h3 className={`text-2xl font-medium mb-8 ${darkMode ? 'text-white' : 'text-zinc-900'
              }`}>
              Your links
            </h3>
            <div className="space-y-3">
              {shortenedUrls.map((item, index) => (
                <div
                  key={item.shortCode}
                  className={`p-6 border-l-4 transition-colors ${darkMode
                    ? 'bg-zinc-800 border-blue-400'
                    : 'bg-white border-red-400'
                    }`}
                  style={{
                    borderRadius: index % 2 === 0 ? '8px 2px 8px 2px' : '2px 8px 2px 8px'
                  }}
                >
                  <div className="flex items-center justify-between">
                    <div className="flex-1 min-w-0 mr-4">
                      <div className="flex items-center mb-2">
                        <div className="w-2 h-2 bg-green-500 rounded-full mr-2"></div>
                        <span className={`text-xs font-bold uppercase tracking-wide ${darkMode ? 'text-green-500' : 'text-green-600'}`}>
                          Active
                        </span>
                        <span className={`text-xs ml-2 ${darkMode ? 'text-zinc-400' : 'text-zinc-500'
                          }`}>
                          {item.remainingShortenings} remainings
                        </span>
                      </div>
                      <p className={`text-xl font-bold mb-1 ${darkMode ? 'text-blue-300' : 'text-red-400'
                        }`}>
                        {item.shortUrl}
                      </p>
                      <p className={`text-sm truncate ${darkMode ? 'text-zinc-400' : 'text-zinc-600'
                        }`}>
                        {item.originalUrl}
                      </p>
                    </div>
                    <div className="flex gap-2">
                      <button
                        onClick={() => copyToClipboard(item.shortUrl, item.shortCode)}
                        className={`p-3 font-black transition-all transform hover:scale-110 ${copiedId === item.shortCode
                          ? 'bg-green-500 text-white'
                          : 'bg-zinc-200 text-zinc-700 hover:bg-green-500 hover:text-white'
                          }`}
                        style={{ borderRadius: '6px 2px 6px 2px' }}
                      >
                        {copiedId === item.shortCode ? (
                          <Check size={16} />
                        ) : (
                          <Copy size={16} />
                        )}
                      </button>
                      <button
                        onClick={() => window.open(item.originalUrl, "_blank")}
                        className="p-3 bg-zinc-200 text-zinc-700 hover:bg-zinc-700 hover:text-white transition-all transform hover:scale-110"
                        style={{ borderRadius: '2px 6px 2px 6px' }}
                      >
                        <ExternalLink size={16} />
                      </button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </main>

      <footer className={`mt-auto py-8 border-t-2 border-dashed ${darkMode ? 'border-zinc-700' : 'border-zinc-300'
        }`}>
        <div className="max-w-4xl mx-auto px-6 text-center">
          <p className={`text-sm font-medium ${darkMode ? 'text-zinc-400' : 'text-zinc-600'
            }`}>
            Zipply © 2025
          </p>
        </div>
      </footer>

      <LoginModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        darkMode={darkMode}
      />
    </div>
  )
}