import { useRef, useState } from 'react';
import { motion, useInView } from 'framer-motion';
import { ArrowRight, Mail, Sparkles } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';

export function Footer() {
  const ref = useRef(null);
  const isInView = useInView(ref, { once: true, margin: '-100px' });
  const [email, setEmail] = useState('');
  const [isSubmitted, setIsSubmitted] = useState(false);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (email) {
      setIsSubmitted(true);
      setTimeout(() => {
        setIsSubmitted(false);
        setEmail('');
      }, 3000);
    }
  };

  return (
    <footer ref={ref} className="relative py-24 px-6 overflow-hidden">
      {/* Large Background Text */}
      <motion.div
        initial={{ opacity: 0, y: 100 }}
        animate={isInView ? { opacity: 1, y: 0 } : {}}
        transition={{ duration: 1, ease: [0.16, 1, 0.3, 1] }}
        className="absolute inset-0 flex items-center justify-center pointer-events-none z-0"
      >
        <h2 
          className="text-[15vw] font-bold text-white/[0.02] whitespace-nowrap select-none"
          style={{ fontFamily: 'Geist Sans, Inter, sans-serif' }}
        >
          TaskWave AI
        </h2>
      </motion.div>

      {/* Gradient Overlay */}
      <div className="absolute inset-0 bg-gradient-to-t from-black via-black/95 to-transparent z-0" />

      {/* Content */}
      <div className="max-w-4xl mx-auto relative z-10">
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.8, ease: [0.16, 1, 0.3, 1] }}
          className="text-center mb-12"
        >
          {/* Icon */}
          <motion.div
            className="inline-flex items-center justify-center w-16 h-16 rounded-2xl bg-gradient-to-r from-indigo-500 to-purple-600 mb-8"
            animate={{
              boxShadow: [
                '0 0 20px rgba(99, 102, 241, 0.4)',
                '0 0 40px rgba(99, 102, 241, 0.6)',
                '0 0 20px rgba(99, 102, 241, 0.4)',
              ],
            }}
            transition={{ duration: 3, repeat: Infinity }}
          >
            <Sparkles className="w-8 h-8 text-white" />
          </motion.div>

          {/* Headline */}
          <h2 className="text-3xl sm:text-4xl md:text-5xl font-bold mb-6 leading-tight">
            Не будьте последним, кто пишет задачи{' '}
            <span className="gradient-text">вручную</span>.
          </h2>

          {/* Subheadline */}
          <p className="text-white/60 text-lg max-w-2xl mx-auto">
            Станьте частью первого потока пользователей и получите Pro-версию навсегда.
          </p>
        </motion.div>

        {/* Subscription Form */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.8, delay: 0.2, ease: [0.16, 1, 0.3, 1] }}
          className="max-w-md mx-auto"
        >
          <form onSubmit={handleSubmit} className="relative">
            <div className="glass-card p-2 flex flex-col sm:flex-row gap-3">
              <div className="relative flex-1">
                <Mail className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-white/40" />
                <Input
                  type="email"
                  placeholder="Ваш e-mail"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="pl-12 h-12 bg-white/5 border-white/10 text-white placeholder:text-white/40 focus:border-indigo-500 focus:ring-indigo-500/20"
                  required
                />
              </div>
              <Button
                type="submit"
                disabled={isSubmitted}
                className="h-12 px-6 bg-gradient-to-r from-indigo-500 to-purple-600 hover:from-indigo-600 hover:to-purple-700 text-white font-semibold rounded-xl glow-button"
              >
                {isSubmitted ? (
                  <span className="flex items-center gap-2">
                    <motion.div
                      initial={{ scale: 0 }}
                      animate={{ scale: 1 }}
                      className="w-5 h-5 rounded-full bg-white/20 flex items-center justify-center"
                    >
                      <Sparkles className="w-3 h-3" />
                    </motion.div>
                    Вы в клубе!
                  </span>
                ) : (
                  <span className="flex items-center gap-2">
                    Вступить в клуб
                    <ArrowRight className="w-4 h-4" />
                  </span>
                )}
              </Button>
            </div>

            {/* Ripple Effect on Submit */}
            {isSubmitted && (
              <motion.div
                className="absolute inset-0 rounded-3xl border-2 border-indigo-500"
                initial={{ scale: 1, opacity: 1 }}
                animate={{ scale: 1.1, opacity: 0 }}
                transition={{ duration: 0.5 }}
              />
            )}
          </form>

          {/* Trust Indicators */}
          <motion.p
            initial={{ opacity: 0 }}
            animate={isInView ? { opacity: 1 } : {}}
            transition={{ duration: 0.8, delay: 0.4 }}
            className="text-center text-white/40 text-sm mt-6"
          >
            Без спама. Отписаться можно в любой момент.
          </motion.p>
        </motion.div>

        {/* Bottom Bar */}
        <motion.div
          initial={{ opacity: 0 }}
          animate={isInView ? { opacity: 1 } : {}}
          transition={{ duration: 0.8, delay: 0.5 }}
          className="mt-20 pt-8 border-t border-white/10"
        >
          <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
            {/* Logo */}
            <div className="flex items-center gap-2">
              <div className="w-8 h-8 rounded-lg bg-gradient-to-r from-indigo-500 to-purple-600 flex items-center justify-center">
                <Sparkles className="w-4 h-4 text-white" />
              </div>
              <span className="font-bold text-white">TaskWave AI</span>
            </div>

            {/* Links */}
            <div className="flex items-center gap-6 text-sm text-white/50">
              <a href="#" className="hover:text-white transition-colors">Политика конфиденциальности</a>
              <a href="#" className="hover:text-white transition-colors">Условия использования</a>
            </div>

            {/* Copyright */}
            <p className="text-sm text-white/40">
              © 2024 TaskWave AI. Все права защищены.
            </p>
          </div>
        </motion.div>
      </div>
    </footer>
  );
}
