/** @type {import('tailwindcss').Config} */
module.exports = {
  mode: 'jit',
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        tgbut: "var(--tg-theme-button-color)",
        tgtxt: "var(--tg-theme-text-color)",
        tgsecback: "var(--tg-theme-secondary-background-color)",
        tgactxt: "var(--tg-theme-accent-text-color)",
        tgsubtxt: "var(--tg-theme-subtitle-text-color)",
        tghint: "var(--tg-theme-hint-color)",
        tgback: "var(--tg-theme-bg-color)",
        tgheaderbg: "var(--tg-header-color)"
      },
      keyframes: {
        fadein: {
          '0%': { opacity: 0 },
          '100%': { opacity: 1 }
        },
        fadeinjelly: { 
          '0%': { transform: 'scale3d(1, 1, 1)' },
          '10%': { transform: 'scale3d(0.90, 1.10, 1)' },
          '20%': { transform: 'scale3d(1.10, 0.90, 1)' },
          '30%': { transform: 'scale3d(0.95, 1.05, 1)' },
          '50%': { transform: 'scale3d(1.03, 0.97, 1)' },
          '75%': { transform: 'scale3d(0.99, 1.01, 1)' },
          '100%': { transform: 'scale3d(1, 1, 1)' }
        }
      },
      animation: {
        fadein: 'fadein 0.5s ease',
        fadeinjelly: 'fadeinjelly 1.5s ease'
      }
    },
  },
  plugins: []
};

