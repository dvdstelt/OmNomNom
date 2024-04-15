import { useEffect, useState } from "react";

export function useLoadData(fn, id) {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      setLoading(true);
      setData(await fn(id));
      setLoading(false);
    }

    load();
  }, [id]);

  return { data, loading };
}
