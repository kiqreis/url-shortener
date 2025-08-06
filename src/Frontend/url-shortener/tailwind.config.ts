import type { Config } from "tailwindcss"

const config: Config = {
    content: [
        "./pages/**/*.{js,ts,jsx,tsx,mdx}",
        "./components/**/*.{js,ts,jsx,tsx,mdx}",
        "./app/**/*.{js,ts,jsx,tsx,mdx}",
    ],
    theme: {
        extend: {
            container: {
                center: true,
                padding: "1rem",
                screens: {
                    "2xl": "1400px",
                },
            },
            fontFamily: {
                sans: ['var(--font-nunito)', 'Helvetica', 'sans-serif'],
                mono: ['var(--font-source-code-pro)', 'monospace'],
            },
        },
    },
    plugins: [],
}

export default config