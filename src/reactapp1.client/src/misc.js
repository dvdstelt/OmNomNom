import { useEffect, useState } from "react";

export function useLoadData(fn, id, callback) {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      setLoading(true);
      const _data = await fn(id);
      setData(_data);
      setLoading(false);
      callback && callback(_data);
    }

    load();
  }, [id]);

  return { data, loading };
}

/**
 *
 * @param {string} key
 * @returns {[string | null, (value: string | null) => void]}
 */
export function useLocalStorage(key) {
  const [item, setItem] = useState(null);

  useEffect(() => {
    setItem(localStorage.getItem(key));
  }, [key]);

  function setLocal(value) {
    localStorage.setItem(key, value);
    setItem(value);
  }

  return [item, setLocal];
}
