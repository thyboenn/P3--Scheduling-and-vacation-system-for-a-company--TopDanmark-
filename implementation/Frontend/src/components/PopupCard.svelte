<script lang="ts">
  import { Dialog } from "attractions";

  export let show: boolean;
  export let title: string;
  export let closeCallback: () => void;
</script>

<div class="dialog-div" class:hide-dialog={!show} class:show-dialog={show}>
  <Dialog {title} {closeCallback}>
    <div slot="title-icon"><slot name="title-icon" /></div>
    <div class="content">
      <slot />
    </div>
  </Dialog>
</div>

<style lang="scss">
  .show-dialog {
    bottom: 0;
  }

  .hide-dialog {
    bottom: -100vh;
  }
  .dialog-div {
    transition: bottom 0.25s cubic-bezier(0.82, 0.085, 0.395, 0.895);
    margin: 1em;
    max-width: calc(100% - 2em);
    position: fixed;
    right: 0;
  }
</style>
